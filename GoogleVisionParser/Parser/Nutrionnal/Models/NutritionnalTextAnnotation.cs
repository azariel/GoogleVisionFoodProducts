using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleVisionParser.Parser.Nutrionnal.Models
{
    public class NutritionnalTextAnnotation
    {
        public string locale {get;set; }
        public string description {get;set; }
        public BoundingPoly boundingPoly {get;set; }
    }
}
