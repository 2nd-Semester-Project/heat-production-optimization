using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HeatOptimiser
{
    public static class SettingsManager
    {
        private static IConfiguration Configuration { get; set; }

        static SettingsManager()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("data/appsettings.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public static string GetSetting(string settingName)
        {
            return Configuration?[settingName] ?? string.Empty;
        }
        public static void SaveSetting(string settingName, string settingValue)
        {
            // Load existing settings
            var settings = new Dictionary<string, string>();
            if (File.Exists("data/appsettings.json"))
            {
                settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("appsettings.json"));
            }

            // Update setting
            settings![settingName] = settingValue;

            // Save settings
            File.WriteAllText("data/appsettings.json", JsonConvert.SerializeObject(settings, Formatting.Indented));
        }
    }
}