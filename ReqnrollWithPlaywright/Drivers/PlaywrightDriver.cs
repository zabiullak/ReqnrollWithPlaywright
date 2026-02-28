using Microsoft.Playwright;
using Serilog;

namespace ReqnrollWithPlaywright.Drivers
{
    public class PlaywrightDriver
    {
        public IPage Page { get; private set; } = null!;

        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;

        public async Task InitializeAsync()
        {
            Log.Information("Launching Chromium browser");
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });
            _context = await _browser.NewContextAsync();
            Page = await _context.NewPageAsync();
            Log.Information("Browser launched and new page created successfully");
        }

        public async Task CaptureScreenshotAsync(string fileName)
        {
            var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));
            var screenshotDir = Path.Combine(projectRoot, "Screenshots");
            Directory.CreateDirectory(screenshotDir);
            var screenshotPath = Path.Combine(screenshotDir, $"{fileName}.png");
            Log.Warning("Step failed — capturing screenshot: {ScreenshotPath}", screenshotPath);
            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });
            Log.Information("Screenshot saved: {FileName}.png", fileName);
        }

        public async Task DisposeAsync()
        {
            Log.Information("Closing browser");
            try
            {
                if (_context is not null)
                    await _context.CloseAsync();

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
