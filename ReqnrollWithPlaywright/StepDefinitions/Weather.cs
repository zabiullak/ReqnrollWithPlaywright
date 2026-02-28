using Microsoft.Playwright;
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
            await _playwrightDriver.Page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded
            });
        }

        [When("i input the location {string}")]
        public async Task WhenIInputTheLocation(string location)
        {
            await _playwrightDriver.Page.Locator(WeatherPageLocators.SearchInput).FillAsync(location);
        }

        [When("click search")]
        public async Task WhenClickSearch()
        {
            await _playwrightDriver.Page.GetByTitle(WeatherPageLocators.SearchBtn).ClickAsync();
            //await _playwrightDriver.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        [Then("i see current weather for {string}")]
        public async Task ThenISeeCurrentWeatherFor(string city)
        {
            await Assertions.Expect(
                _playwrightDriver.Page.Locator(WeatherPageLocators.LocationHeading)
            ).ToContainTextAsync(city);
        }
    }
}
