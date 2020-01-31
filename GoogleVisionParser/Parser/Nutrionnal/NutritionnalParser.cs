using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GoogleVisionParser.Parser.Nutrionnal.Models;
using Newtonsoft.Json;
using static GoogleVisionParser.Parser.Nutrionnal.Models.NutrionnalPanelModel;

namespace GoogleVisionParser.Parser.Nutrionnal
{
    public class NutritionnalParser : IFoodParser
    {
        private bool ValidateJsonIsNutritionnalData(string aRawJsonData)
        {
            // TODO: validations
            return true;
        }

        /// <summary>
        /// Parse json raw data obtained from google vision
        /// </summary>
        public List<NutritionalFoodView> Parse(string aRawJsonData, out FoodParserReport aFoodParserReport)
        {
            aFoodParserReport = new FoodParserReport();

            if (string.IsNullOrWhiteSpace(aRawJsonData))
                return null;

            if (!ValidateJsonIsNutritionnalData(aRawJsonData))
                return null;

            NutrionnalPanelModel _DeserializedModel;

            try
            {
                _DeserializedModel = JsonConvert.DeserializeObject<NutrionnalPanelModel>(aRawJsonData);
            }
            catch (Exception _Ex)
            {
                Console.WriteLine("Invalid input file.");
                File.AppendAllText(Constants.ERROR_FILE, $"Input file was invalid. Ex: [{_Ex.Message}], [{_Ex.StackTrace}], [{_Ex.InnerException?.Message}], [{_Ex.InnerException?.StackTrace}]");
                return null;
            }

            if (_DeserializedModel.responses.Count > 1)
            {
                Console.WriteLine("Invalid input file. Multiple responses found.");
                File.AppendAllText(Constants.ERROR_FILE, $"Input file was invalid. Multiple responses found.");
                return null;
            }

            NutrionnalPanelModel.Response _WorkItem = _DeserializedModel.responses[0];

            // Order by X
            _WorkItem.textAnnotations = _WorkItem.textAnnotations.OrderBy(o => o.boundingPoly.vertices.FirstOrDefault()?.x).ToList();

            //IEnumerable<IGrouping<int?, TextAnnotation>> a = _WorkItem.textAnnotations.GroupBy(o =>
            //{
            //    return o.boundingPoly.vertices.FirstOrDefault()?.y / Constants.NOISE;
            //});

            List<List<TextAnnotation>> b = new List<List<TextAnnotation>>();

            foreach (var o in _WorkItem.textAnnotations.Skip(ConfigManager.GetConfig().fSKIP_DATA_ELEMENT_NB))
            {
                bool _Found = false;
                foreach (List<TextAnnotation> item in b)
                {
                    foreach (TextAnnotation item2 in item)
                    {
                        if (o.boundingPoly.vertices.FirstOrDefault()?.y > item2.boundingPoly.vertices.FirstOrDefault()?.y - ConfigManager.GetConfig().fNOISE &&
                            o.boundingPoly.vertices.FirstOrDefault()?.y < item2.boundingPoly.vertices.FirstOrDefault()?.y + ConfigManager.GetConfig().fNOISE)
                        {
                            item.Add(o);
                            _Found = true;
                            break;
                        }

                        if (_Found)
                            break;
                    }

                    if (_Found)
                        break;
                }

                if (_Found)
                    continue;

                b.Add(new List<TextAnnotation>() { o });
                //return o.boundingPoly.vertices.FirstOrDefault()?.y / Constants.NOISE;
            }

            // Order

            b = b.OrderBy(o => o[0].boundingPoly.vertices.FirstOrDefault()?.y).ToList();

            //List<string> _String = new List<string>();
            //foreach (var item in b)
            //{
            //    string _Content = "";
            //    foreach (var item2 in item)
            //    {
            //        _Content += $" {item2.description} ({string.Join("/", item2.boundingPoly.vertices.Select(s => s.x))}---{string.Join("/", item2.boundingPoly.vertices.Select(s => s.y))})";
            //    }

            //    _String.Add($"{_Content}");
            //}

            var _NutritionalFoodViews = new List<NutritionalFoodView>();
            foreach (List<TextAnnotation> _v in b)
            {
                FillUp(_v, _NutritionalFoodViews);
            }

            return _NutritionalFoodViews;
        }

        private void FillUp(List<TextAnnotation> aV, List<NutritionalFoodView> aListToFillUp)
        {
            var _a = ExtractFoodViewFromNutritionalData(aV, out var _remainingData);

            if (_a != null)
            {
                if (_a.Name != "")
                    aListToFillUp.Add(_a);
                else if (_a.Amount != "" || _a.Percentage != "?")
                {
                    var _LastOnStack = aListToFillUp.LastOrDefault();

                    if (_LastOnStack != null)
                    {
                        if (_LastOnStack.Amount == "" && _a.Amount != "")
                            _LastOnStack.Amount = _a.Amount;

                        if (_LastOnStack.Percentage == "?" && _a.Percentage != "?")
                            _LastOnStack.Percentage = _a.Percentage;

                        // Amount is percentage
                        if (_LastOnStack.Percentage == "?" && _a.Amount != "?" && _a.Amount != "" && _a.TypeAmount == "%")
                            _LastOnStack.Percentage = _a.Amount;
                    }
                }
            }

            if (_remainingData != null && _remainingData.Count > 0)
            {
                FillUp(_remainingData, aListToFillUp);
            }
        }

        private NutritionalFoodView ExtractFoodViewFromNutritionalData(List<TextAnnotation> aVendetta, out List<TextAnnotation> aRemainingData)
        {
            aRemainingData = null;
            string _Name = "";
            string _Amount = "";
            string _Percentage = "";
            string _TypeAmount = "";

            bool _AmountFound = false;
            bool _PercFound = false;
            bool _PercConfirmed = false;
            List<TextAnnotation> _remainingTextAnnotations = new List<TextAnnotation>();
            foreach (TextAnnotation item in aVendetta)
            {
                if (!_AmountFound)
                {
                    string _AmNb = ExtractNumberFromString(item.description);
                    if (!string.IsNullOrWhiteSpace(_AmNb))
                    {
                        float.TryParse(_AmNb, out float _Am);
                        _Amount = _Am.ToString(CultureInfo.InvariantCulture);
                        _TypeAmount = item.description.Replace(_AmNb, "");

                        _AmountFound = true;
                    }
                    else
                    {
                        _Name += item.description;
                    }
                }
                else if (!_PercFound)
                {
                    if (float.TryParse(item.description, out float _descriptionAsNumber))
                    {
                        _Percentage = _descriptionAsNumber.ToString(CultureInfo.InvariantCulture);
                        _PercFound = true;
                    }
                    else
                    {
                        _remainingTextAnnotations.Add(item);
                    }
                }
                else
                {
                    if (item.description.Trim().StartsWith("%"))
                        _PercConfirmed = true;
                    else
                        _remainingTextAnnotations.Add(item);
                }
            }

            aRemainingData = _remainingTextAnnotations;

            // get Box
            int? _TopX = null;
            int? _TopY = null;
            int? _BottomX = null;
            int? _BottomY = null;

            foreach (Vertex item in aVendetta.SelectMany(sm => sm.boundingPoly.vertices))
            {
                if(_TopX == null || _TopX > item.x)
                    _TopX = item.x;

                if(_TopY == null || _TopY > item.y)
                    _TopY = item.y;

                if(_BottomX == null || _BottomX < item.x)
                    _BottomX = item.x;

                if(_BottomY == null || _BottomX < item.y)
                    _BottomY = item.y;

            }

            return new NutritionalFoodView
            {
                Name = _Name,
                TypeAmount = _TypeAmount,
                Amount = _Amount,
                Percentage = _PercConfirmed ? _Percentage : "?",
                Locations = new Vertex[]
                {
                    new Vertex(_TopX.Value, _TopY.Value),
                    new Vertex(_BottomX.Value, _TopY.Value),
                    new Vertex(_BottomX.Value, _BottomY.Value),
                    new Vertex(_TopX.Value, _BottomY.Value)
                }
            };
        }

        private string ExtractNumberFromString(string aRaw)
        {
            return Regex.Match(aRaw, @"([-+]?[0-9]*\.?\,?[0-9]+)").Value;
        }
    }
}
