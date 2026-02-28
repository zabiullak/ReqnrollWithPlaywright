using Reqnroll.BoDi;
using ReqnrollWithPlaywright.Drivers;
using ReqnrollWithPlaywright.Pages;

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

        //[BeforeScenario(Order = 3)]
        //public void RegisterPageObjects()
        //{
        //    var driver = _objectContainer.Resolve<PlaywrightDriver>();
        //    _objectContainer.RegisterInstanceAs(new HomePage(driver));
        //    _objectContainer.RegisterInstanceAs(new NewsPage(driver));
        //}
    }
}
