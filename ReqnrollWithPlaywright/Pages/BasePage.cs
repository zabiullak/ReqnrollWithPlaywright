using Microsoft.Playwright;
using Serilog;
using ReqnrollWithPlaywright.Drivers;

namespace ReqnrollWithPlaywright.Pages
{
    public abstract class BasePage
    {
        protected readonly IPage Page;

        protected BasePage(PlaywrightDriver playwrightDriver)
        {
            Page = playwrightDriver.Page;
        }

        public async Task NavigateToAsync(string url)
        {
            Log.Information("Navigating to URL: {Url}", url);
            await Page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });
            Log.Debug("Page loaded: {Url}", url);
        }
    }
}
