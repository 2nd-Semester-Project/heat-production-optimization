using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace HeatOptimiser
{
    public static class SettingsManager
    {
        private static IConfiguration Configuration { get; set; }

        static SettingsManager()
        {
            Configuration = InitializeBuilder();
        }
        public static IConfiguration InitializeBuilder()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("data/appsettings.json", optional: true, reloadOnChange: true);

            try
            {
                Configuration = builder.Build();
            }
            catch (Exception)
            {
                // Delete the file if it exists
                if (File.Exists("data/appsettings.json"))
                {
                    File.Delete("data/appsettings.json");
                }
                builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("data/appsettings.json", optional: true, reloadOnChange: true);
                Configuration = builder.Build();
            }
            return Configuration;
        }

        public static string GetSetting(string settingName)
        {
            try
            {
                Configuration = InitializeBuilder();
                return Configuration?[settingName]! ?? string.Empty;
            }
            catch (Exception)
            {
                Configuration = InitializeBuilder();
                return string.Empty;
            }
        }
        public static void SaveSetting(string settingName, string settingValue)
        {
            // Load existing settings
            var settings = new Dictionary<string, string>();
            string filePath = "data/appsettings.json";
            string directoryPath = Path.GetDirectoryName(filePath)!;
            if (File.Exists(filePath))
            {
                settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(filePath));
            }

            // Update setting
            settings![settingName] = settingValue;

            // Check if directory exists, create if not
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Save settings
            File.WriteAllText(filePath, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }
    }
}