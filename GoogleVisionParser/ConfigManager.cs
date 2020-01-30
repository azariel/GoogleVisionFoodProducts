using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace GoogleVisionParser
{
    public static class ConfigManager
    {
        private static  Config fConfig;

        public static  Config GetConfig()
        {
            if (fConfig == null)
                DeserializeConfig();

            return fConfig;
        }

        private static  void DeserializeConfig()
        {
            if(!File.Exists(Constants.CONFIG_FILE))
                File.WriteAllText(Constants.CONFIG_FILE, JsonConvert.SerializeObject(new Config()));

            string aRaw = File.ReadAllText(Constants.CONFIG_FILE);
            fConfig = JsonConvert.DeserializeObject<Config>(aRaw);
        }
    }
}
