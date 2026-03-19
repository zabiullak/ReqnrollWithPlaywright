using NUnit.Framework;

namespace ReqnrollWithPlaywright.Support
{
    public static class RunSettingsReader
    {
        //private static readonly Lazy<Dictionary<string, string>> _settings = new(LoadSettings);

        //private static Dictionary<string, string> LoadSettings()
        //{
        //    var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        //    string filePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Settings.runsettings"));
        //    try
        //    {
        //        var doc = XDocument.Load(filePath);
        //        foreach (var param in doc.Descendants("Parameter"))
        //        {
        //            var name = param.Attribute("name")?.Value;
        //            var value = param.Attribute("value")?.Value;

        //            if (!string.IsNullOrEmpty(name))
        //                settings[name] = value;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.Fail($"Error loading runsettings file: {ex.Message} \n {ex.StackTrace}");
        //    }
        //    return settings;
        //}
        public static string Get(string key)
        {
            var value = TestContext.Parameters[key];
            Assert.That(value, Is.Not.Null, $"Invalid key '{key}'! Please check the .runsettings file.");
            return value!;
        }
    }
}
