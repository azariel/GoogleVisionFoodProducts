using System;
using System.IO;
using GoogleVisionParser.Parser.Nutrionnal;
using GoogleVisionParser.Parser.Nutrionnal.Models;
using Newtonsoft.Json;

namespace GoogleVisionParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string _RawDataFromFile = File.ReadAllText("DATA.json");
            NutritionnalParser _Parser = new NutritionnalParser();
            var _Result = _Parser.Parse(_RawDataFromFile, out var _Report);

            File.WriteAllText("Json.json", JsonConvert.SerializeObject(_Result));

            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
