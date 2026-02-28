using ReqnrollWithPlaywright.Drivers;

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
        public void GivenINavigateTo(string p0)
        {
            throw new PendingStepException();
        }

        [When("i input the location {string}")]
        public void WhenIInputTheLocation(string p0)
        {
            throw new PendingStepException();
        }

        [When("click search")]
        public void WhenClickSearch()
        {
            throw new PendingStepException();
        }

        [Then("i see current weather for {string}")]
        public void ThenISeeCurrentWeatherFor(string bangalore)
        {
            throw new PendingStepException();
        }

    }
}
