using Reqnroll.BoDi;
using ReqnrollWithPlaywright.Drivers;

namespace ReqnrollWithPlaywright.Support
{
    [Binding]
    public class Dependencies
    {
        private readonly IObjectContainer _objectContainer;

        public Dependencies(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario(Order = 1)]
        public void RegisterScenarioDependencies()
        {
            //_objectContainer.RegisterTypeAs<ChromeDriver, IDriver>();
            //_objectContainer.RegisterTypeAs<PlaywrightDriver, PlaywrightDriver>();
        }
    }
}
