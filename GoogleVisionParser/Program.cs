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
            string _RawDataFromFile = File.ReadAllText("DATA.txt");
            NutritionnalParser _Parser = new NutritionnalParser();
            _Parser.Parse(_RawDataFromFile, out var _Report);

            Console.ReadKey();
        }
    }
}
