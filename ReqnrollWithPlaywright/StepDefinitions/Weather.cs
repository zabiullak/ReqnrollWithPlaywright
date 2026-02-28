using Microsoft.Playwright;
using Serilog;
using ReqnrollWithPlaywright.Drivers;
using ReqnrollWithPlaywright.Locators;

namespace ReqnrollWithPlaywright.StepDefinitions
{
    [Binding]
    public sealed class Weather
    {
        private readonly PlaywrightDriver _playwrightDriver;

        public Weather(PlaywrightDriver playwrightDriver)
        {
            _playwrightDriver = playwrightDriver;
        }

        [Given("i navigate to {string}")]
        public async Task GivenINavigateTo(string url)
        {
            Log.Information("Navigating to URL: {Url}", url);
            await _playwrightDriver.Page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });
            Log.Debug("Page loaded: {Url}", url);
        }

        [When("i input the location {string}")]
        public async Task WhenIInputTheLocation(string location)
        {
            Log.Information("Entering location into search box: {Location}", location);
            await _playwrightDriver.Page.Locator(WeatherPageLocators.SearchInput).FillAsync(location);
        }

        [When("click search")]
        public async Task WhenClickSearch()
        {
            Log.Information("Clicking the search button");
            await _playwrightDriver.Page.GetByTitle(WeatherPageLocators.SearchBtn).ClickAsync();
            Log.Debug("Search submitted");
        }

        [Then("i see current weather for {string}")]
        public async Task ThenISeeCurrentWeatherFor(string city)
        {
            Log.Information("Asserting weather page is shown for: {City}", city);
            await Assertions.Expect(
                _playwrightDriver.Page.Locator(WeatherPageLocators.LocationHeading)
            ).ToContainTextAsync(city);
            Log.Information("Assertion passed — weather page confirmed for: {City}", city);
        }
    }
}
