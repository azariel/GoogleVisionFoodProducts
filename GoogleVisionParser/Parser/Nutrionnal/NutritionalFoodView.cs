using static GoogleVisionParser.Parser.Nutrionnal.Models.NutrionnalPanelModel;

namespace GoogleVisionParser.Parser.Nutrionnal
{
    public class NutritionalFoodView
    {
        public string Name { get; set; }
        public string Amount { get; set; }
        public string TypeAmount { get; set; }
        public string Percentage { get; set; }
        public Vertex[] Locations { get; set; }
    }
}
