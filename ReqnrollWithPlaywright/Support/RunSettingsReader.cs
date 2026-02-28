using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ReqnrollWithPlaywright.Support
{
    public static class RunSettingsReader
    {

        private static Dictionary<string, string> _settings;
        private static bool _isLoaded = false;

        private static void Load()
        {
            _settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string filePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Settings.runsettings"));
            try
            {
                var doc = XDocument.Load(filePath);
                foreach (var param in doc.Descendants("Parameter"))
                {
                    var name = param.Attribute("name")?.Value;
                    var value = param.Attribute("value")?.Value;

                    if (!string.IsNullOrEmpty(name))
                        _settings[name] = value;
                }

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                Assert.Fail($"Error loading runsettings file: {ex.Message} \n {ex.StackTrace}");
            }
        }

        // Get the value by key
        public static string Get(string key)
        {
            if (!_isLoaded)
                Load();
            Assert.That(_settings.ContainsKey(key), Is.True , "Invalid Key! please check the .runsetting file");
            return _settings.TryGetValue(key, out var value) ? value : null;
        }
    }
}
