using FluentAssertions;
using ReqnrollWithPlaywright.Pages;
using Serilog;

namespace ReqnrollWithPlaywright.StepDefinitions
{
    [Binding]
    public sealed class Weather
    {
        private readonly HomePage _homePage;
        private readonly NewsPage _newsPage;

        public Weather(HomePage homePage, NewsPage newsPage)
        {
            _homePage = homePage;
            _newsPage = newsPage;
        }

        [Given("i navigate to {string}")]
        public async Task GivenINavigateTo(string url)
        {
            await _homePage.NavigateToAsync(url);
        }

        [When("i input the location {string}")]
        public async Task WhenIInputTheLocation(string location)
        {
            await _homePage.EnterLocation(location);
        }

        [When("click search")]
        public async Task WhenClickSearch()
        {
            await _homePage.ClickOnSearchbtn();
        }

        [Then("i see current weather for {string}")]
        public async Task ThenISeeCurrentWeatherFor(string city)
        {
            var result = await _homePage.GetCurrentWeatherInfo(city);
            result.Should().Contain(city);
            Log.Information("Assertion passed — weather page confirmed for: {City}", city);
        }

        [When("i click on News link")]
        public async Task WhenIClickOnNewsLink()
        {
            await _homePage.ClickOnNewsLink();
        }

        [Then("i see the News page")]
        public async Task ThenISeeTheNewsPage()
        {
            string response = await _newsPage.GetPageTitle();
            response.Should().Contain("News");
            Log.Information("Assertion passed — News page title contains 'News'");
        }
    }
}
