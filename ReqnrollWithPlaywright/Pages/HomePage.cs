using Microsoft.Playwright;
using ReqnrollWithPlaywright.Drivers;
using Serilog;

namespace ReqnrollWithPlaywright.Pages
{
    public class HomePage : BasePage
    {
        public HomePage(PlaywrightDriver playwrightDriver) : base(playwrightDriver)
        {
        }

        private ILocator SearchBox => Page.Locator("#ls-c-search__input-label");
        private ILocator SearchBtn => Page.GetByTitle("Search for a location");
        private ILocator LocationHeading => Page.Locator("#wr-location-name-id");
        private ILocator NewsLink => Page.Locator(".orb-nav-commercial-news");

        internal async Task EnterLocation(string location)
        {
            await EnterTextAsync(SearchBox, location, "Search box");
        }

        internal async Task ClickOnSearchbtn()
        {
            await ClickAsync(SearchBtn, "Search button");
        }

        internal async Task<string> GetCurrentWeatherInfo(string city)
        {
            Log.Information("Asserting weather page is shown for: {City}", city);
            return await LocationHeading.InnerTextAsync();
        }

        internal async Task ClickOnNewsLink()
        {
            await ClickAsync(NewsLink.First, "News link");
            await Page.WaitForLoadStateAsync(LoadState.Load);
        }
    }
}
