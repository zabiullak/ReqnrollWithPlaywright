using Microsoft.Playwright;
using NUnit.Framework;
using ReqnrollWithPlaywright.Support;
using Serilog;

namespace ReqnrollWithPlaywright.Drivers
{
    public class PlaywrightDriver : IAsyncDisposable
    {
        public IPage Page { get; private set; } = null!;

        private static readonly string ProjectRoot =
            Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));

        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private bool _traceSaved;

        public async Task InitializeAsync()
        {
            bool headless = TestContext.Parameters.Get<bool>("Headless", true);
            string browserName = TestContext.Parameters.Get("Browser", "Chromium")!;

            Log.Information("Launching {Browser} browser (Headless: {Headless})", browserName, headless);
            _playwright = await Playwright.CreateAsync();

            var launchOptions = new BrowserTypeLaunchOptions { Headless = headless };

            _browser = browserName.ToUpperInvariant() switch
            {
                "FIREFOX" => await _playwright.Firefox.LaunchAsync(launchOptions),
                "WEBKIT"  => await _playwright.Webkit.LaunchAsync(launchOptions),
                _         => await _playwright.Chromium.LaunchAsync(launchOptions),
            };

            _context = await _browser.NewContextAsync();
            Page = await _context.NewPageAsync();

            float timeout = TestContext.Parameters.Get<float>("Timeout", 30000);
            Page.SetDefaultTimeout(timeout);
            Log.Debug("Default action timeout set to {Timeout}ms", timeout);

            await _context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
            Log.Information("Browser launched, new page created and tracing started");
        }

        public async Task<string> CaptureScreenshotAsync(string fileName)
        {
            var screenshotDir = Path.Combine(ProjectRoot, "Screenshots");
            Directory.CreateDirectory(screenshotDir);
            var screenshotPath = Path.Combine(screenshotDir, $"{fileName}.png");
            Log.Warning("Step failed - capturing screenshot: {ScreenshotPath}", screenshotPath);
            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });
            Log.Information("Screenshot saved: {FileName}.png", fileName);
            return screenshotPath;
        }

        public async Task SaveTraceAsync(string fileName)
        {
            if (_context is null || _traceSaved) return;

            var traceDir = Path.Combine(ProjectRoot, "Traces");
            Directory.CreateDirectory(traceDir);
            var tracePath = Path.Combine(traceDir, $"{fileName}.zip");

            Log.Warning("Saving trace to: {TracePath}", tracePath);
            await _context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
            _traceSaved = true;
            Log.Information("Trace saved: {FileName}.zip", fileName);
        }

        public async ValueTask DisposeAsync()
        {
            Log.Information("Closing browser");
            try
            {
                if (_context is not null)
                {
                    if (!_traceSaved)
                        await _context.Tracing.StopAsync(new TracingStopOptions());

                    await _context.CloseAsync();
                }

                if (_browser is not null)
                    await _browser.CloseAsync();
            }
            finally
            {
                _playwright?.Dispose();
            }
            Log.Information("Browser closed and resources disposed");
            GC.SuppressFinalize(this);
        }
    }
}
