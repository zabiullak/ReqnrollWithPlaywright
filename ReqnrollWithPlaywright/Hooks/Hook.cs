using ReqnrollWithPlaywright.Drivers;

namespace ReqnrollWithPlaywright.Hooks
{
    [Binding]
    public sealed class Hook
    {
        private readonly PlaywrightDriver _playwrightDriver;

        public Hook(PlaywrightDriver playwrightDriver)
        {
            _playwrightDriver = playwrightDriver;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            await _playwrightDriver.InitializeAsync();
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            await _playwrightDriver.DisposeAsync();
        }
    }
}
