using Allure.Net.Commons;
using Serilog;
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
            Log.Information("---------- Scenario Started: {Title} | Tags: [{Tags}] ----------",
                _scenarioContext.ScenarioInfo.Title,
                string.Join(", ", _scenarioContext.ScenarioInfo.Tags));

            await _playwrightDriver.InitializeAsync();
        }

        [AfterStep]
        public async Task AfterStep()
        {
            var stepInfo = _scenarioContext.StepContext.StepInfo;

            if (_scenarioContext.TestError != null)
            {
                Log.Error("FAILED | {StepType} {StepText} | Error: {ErrorMessage}",
                    stepInfo.StepDefinitionType,
                    stepInfo.Text,
                    _scenarioContext.TestError.Message);

                var scenarioTitle = _scenarioContext.ScenarioInfo.Title.Replace(" ", "_");
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var fileName = $"{scenarioTitle}_{timestamp}";

                await _playwrightDriver.SaveTraceAsync(fileName);
                var screenshotPath = await _playwrightDriver.CaptureScreenshotAsync(fileName);
                AllureApi.AddAttachment("Failure Screenshot", "image/png", screenshotPath);
            }
            else
            {
                Log.Information("PASSED | {StepType} {StepText}",
                    stepInfo.StepDefinitionType,
                    stepInfo.Text);
            }
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            var result = _scenarioContext.TestError == null ? "PASSED" : "FAILED";
            Log.Information("---------- Scenario {Result}: {Title} ----------",
                result,
                _scenarioContext.ScenarioInfo.Title);

            await _playwrightDriver.DisposeAsync().AsTask();
        }
    }
}
