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
            try
            {
                string _RawDataFromFile = File.ReadAllText("DATA.json");
                NutritionnalParser _Parser = new NutritionnalParser();
                var _Result = _Parser.Parse(_RawDataFromFile, out var _Report);

                File.WriteAllText("Json.json", JsonConvert.SerializeObject(_Result));

                Console.WriteLine("Done.");
                Console.ReadKey();
            }
            catch (Exception _Ex)
            {
               File.AppendAllText(Constants.ERROR_FILE, $"Error. Ex: [{_Ex.Message}], [{_Ex.StackTrace}], [{_Ex.InnerException?.Message}], [{_Ex.InnerException?.StackTrace}]");
            }
        }
    }
}
