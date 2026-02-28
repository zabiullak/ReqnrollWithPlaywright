using Microsoft.Playwright;
using ReqnrollWithPlaywright.Drivers;

namespace ReqnrollWithPlaywright.Pages
{
    public class NewsPage : BasePage
    {
        public NewsPage(PlaywrightDriver playwrightDriver) : base(playwrightDriver)
        {
        }

        internal async Task<string> GetPageTitle()
        {
            return await Page.TitleAsync();
        }
    }
}
