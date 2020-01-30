using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        public void Parse(string aRawJsonData, out FoodParserReport aFoodParserReport)
        {
            aFoodParserReport = new FoodParserReport();

            if (string.IsNullOrWhiteSpace(aRawJsonData))
                return;

            if (!ValidateJsonIsNutritionnalData(aRawJsonData))
                return;

            NutrionnalPanelModel _DeserializedModel;

            try
            {
                _DeserializedModel = JsonConvert.DeserializeObject<NutrionnalPanelModel>(aRawJsonData);
            }
            catch (Exception _Ex)
            {
                Console.WriteLine("Invalid input file.");
                File.AppendAllText(Constants.ERROR_FILE, $"Input file was invalid. Ex: [{_Ex.Message}], [{_Ex.StackTrace}], [{_Ex.InnerException?.Message}], [{_Ex.InnerException?.StackTrace}]");
                return;
            }

            if (_DeserializedModel.responses.Count > 1)
            {
                Console.WriteLine("Invalid input file. Multiple responses found.");
                File.AppendAllText(Constants.ERROR_FILE, $"Input file was invalid. Multiple responses found.");
                return;
            }

            // Ordering
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

            b = b.OrderBy(o=>o[0].boundingPoly.vertices.FirstOrDefault()?.y).ToList();

            List<string> _String = new List<string>();
            foreach (var item in b)
            {
                string _Content = "";
                foreach (var item2 in item)
                {
                    _Content += $" {item2.description} ({string.Join("/", item2.boundingPoly.vertices.Select(s => s.x))}---{string.Join("/", item2.boundingPoly.vertices.Select(s => s.y))})";
                }

                _String.Add($"{_Content}");
            }
        }
    }
}
