using Microsoft.Playwright;
using ReqnrollWithPlaywright.Support;
using Serilog;

namespace ReqnrollWithPlaywright.Drivers
{
    public class PlaywrightDriver
    {
        public IPage Page { get; private set; } = null!;

        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private bool _traceSaved;

        public async Task InitializeAsync()
        {
            bool headless = bool.Parse(RunSettingsReader.Get("Headless"));
            Log.Information("Launching Chromium browser (Headless: {Headless})", headless);
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = headless
            });
            _context = await _browser.NewContextAsync();
            Page = await _context.NewPageAsync();

            await _context.Tracing.StartAsync(new TracingStartOptions
            {
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
            Log.Information("Browser launched, new page created and tracing started");
        }

        public async Task CaptureScreenshotAsync(string fileName)
        {
            var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));
            var screenshotDir = Path.Combine(projectRoot, "Screenshots");
            Directory.CreateDirectory(screenshotDir);
            var screenshotPath = Path.Combine(screenshotDir, $"{fileName}.png");
            Log.Warning("Step failed - capturing screenshot: {ScreenshotPath}", screenshotPath);
            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });
            Log.Information("Screenshot saved: {FileName}.png", fileName);
        }

        public async Task SaveTraceAsync(string fileName)
        {
            if (_context is null || _traceSaved) return;

            var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));
            var traceDir = Path.Combine(projectRoot, "Traces");
            Directory.CreateDirectory(traceDir);
            var tracePath = Path.Combine(traceDir, $"{fileName}.zip");

            Log.Warning("Saving trace to: {TracePath}", tracePath);
            await _context.Tracing.StopAsync(new TracingStopOptions { Path = tracePath });
            _traceSaved = true;
            Log.Information("Trace saved: {FileName}.zip", fileName);
        }

        public async Task DisposeAsync()
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
        }
    }
}
