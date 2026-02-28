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
            Log.Information("Entering location into search box: {Location}", location);
            await SearchBox.FillAsync(location);
        }

        internal async Task ClickOnSearchbtn()
        {
            Log.Information("Clicking the search button");
            await SearchBtn.ClickAsync();
            Log.Debug("Search submitted");
        }

        internal async Task<ILocator> GetCurrentWeatherInfo(string city)
        {
            Log.Information("Asserting weather page is shown for: {City}", city);
            return LocationHeading;
        }

        internal async Task ClickOnNewsLink()
        {
           Log.Information("Clicking on the News link");
            await NewsLink.First.ClickAsync();
            await Page.WaitForLoadStateAsync(LoadState.Load);
            Log.Debug("News link clicked, navigating to News page");
        }
    }
}
