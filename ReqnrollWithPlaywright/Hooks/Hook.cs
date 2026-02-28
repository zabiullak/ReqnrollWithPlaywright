using ReqnrollWithPlaywright.Drivers;

namespace ReqnrollWithPlaywright.Hooks
{
    [Binding]
    public sealed class Hook
    {
        private readonly PlaywrightDriver _playwrightDriver;
        private readonly ScenarioContext _scenarioContext;

        public Hook(PlaywrightDriver playwrightDriver, ScenarioContext scenarioContext)
        {
            _playwrightDriver = playwrightDriver;
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario(Order = 2)]
        public async Task BeforeScenario()
        {
            await _playwrightDriver.InitializeAsync();
        }

        [AfterStep]
        public async Task AfterStep()
        {
            if (_scenarioContext.TestError != null)
            {
                var scenarioTitle = _scenarioContext.ScenarioInfo.Title.Replace(" ", "_");
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                await _playwrightDriver.CaptureScreenshotAsync($"{scenarioTitle}_{timestamp}");
            }
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            await _playwrightDriver.DisposeAsync();
        }
    }
}
