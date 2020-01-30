using System;
using System.Collections.Generic;
using System.Text;

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

            if(!ValidateJsonIsNutritionnalData(aRawJsonData))
                return;


        }
    }
}
