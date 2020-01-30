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
                File.AppendAllText(Constants.LOG_FILE, $"Input file was invalid. Ex: [{_Ex.Message}], [{_Ex.StackTrace}], [{_Ex.InnerException?.Message}], [{_Ex.InnerException?.StackTrace}]");
                return;
            }

            if (_DeserializedModel.responses.Count > 0)
            {
                Console.WriteLine("Invalid input file. Multiple responses found.");
                File.AppendAllText(Constants.LOG_FILE, $"Input file was invalid. Multiple responses found.");
                return;
            }

            // Ordering
            NutrionnalPanelModel.Response _WorkItem = _DeserializedModel.responses[0];

            List<TextAnnotation> _OrderedByY = new List<TextAnnotation>();

            var a = _WorkItem.textAnnotations.GroupBy(o =>
            {
                //return o.boundingPoly.vertices.FirstOrDefault()?.y    
            });

            _OrderedByY = _WorkItem.textAnnotations.OrderBy(o => o.boundingPoly.vertices.FirstOrDefault()?.y).ToList();

        }
    }
}
