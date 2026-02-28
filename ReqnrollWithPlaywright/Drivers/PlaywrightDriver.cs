using Microsoft.Playwright;

namespace ReqnrollWithPlaywright.Drivers
{
    public class PlaywrightDriver
    {
        public IPage Page { get; private set; } = null!;

        private IPlaywright? _playwright;
        private IBrowser? _browser;

        public async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });
            Page = await _browser.NewPageAsync();
        }

        public async Task DisposeAsync()
        {
            if (_browser is not null)
                await _browser.CloseAsync();

            _playwright?.Dispose();
        }
    }
}
