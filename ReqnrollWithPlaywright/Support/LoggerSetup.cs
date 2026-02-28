using Serilog;

namespace ReqnrollWithPlaywright.Support
{
    [Binding]
    public class LoggerSetup
    {
        [BeforeTestRun]
        public static void InitializeLogger()
        {
            var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));
            var logDir = Path.Combine(projectRoot, "Logs");
            Directory.CreateDirectory(logDir);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: Path.Combine(logDir, "test-log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("========== Test Run Started ==========");
        }

        [AfterTestRun]
        public static void TearDownLogger()
        {
            Log.Information("========== Test Run Completed ==========");
            Log.CloseAndFlush();
        }
    }
}
