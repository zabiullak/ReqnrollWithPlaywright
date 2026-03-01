using Microsoft.Playwright;
using Serilog;
using ReqnrollWithPlaywright.Drivers;

namespace ReqnrollWithPlaywright.Pages
{
    public abstract class BasePage
    {
        protected readonly IPage Page;

        protected BasePage(PlaywrightDriver playwrightDriver)
        {
            Page = playwrightDriver.Page;
        }

        // -------------------------------------------------------------------------
        // Navigation
        // -------------------------------------------------------------------------

        public async Task NavigateToAsync(string url)
        {
            Log.Information("Navigating to: {Url}", url);
            await Page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
            Log.Debug("Page loaded: {Url}", url);
        }

        // -------------------------------------------------------------------------
        // Click actions
        // -------------------------------------------------------------------------

        /// <summary>Single left-click on the element.</summary>
        protected async Task ClickAsync(ILocator locator, string description = "")
        {
            Log.Information("Click — {Target}", Label(locator, description));
            await locator.ClickAsync();
        }

        /// <summary>Double-click on the element.</summary>
        protected async Task DoubleClickAsync(ILocator locator, string description = "")
        {
            Log.Information("Double-click — {Target}", Label(locator, description));
            await locator.DblClickAsync();
        }

        /// <summary>Right-click (context-menu click) on the element.</summary>
        protected async Task RightClickAsync(ILocator locator, string description = "")
        {
            Log.Information("Right-click — {Target}", Label(locator, description));
            await locator.ClickAsync(new LocatorClickOptions { Button = MouseButton.Right });
        }

        // -------------------------------------------------------------------------
        // Text / Input actions
        // -------------------------------------------------------------------------

        /// <summary>Clear the field and fill it with <paramref name="text"/> in one shot (no key events).</summary>
        protected async Task EnterTextAsync(ILocator locator, string text, string description = "")
        {
            Log.Information("Enter text '{Text}' — {Target}", text, Label(locator, description));
            await locator.FillAsync(text);
        }

        /// <summary>Simulate real keystroke-by-keystroke typing into the element.</summary>
        protected async Task TypeTextAsync(ILocator locator, string text, string description = "")
        {
            Log.Information("Type text '{Text}' — {Target}", text, Label(locator, description));
            await locator.PressSequentiallyAsync(text);
        }

        /// <summary>Clear the current value of the element.</summary>
        protected async Task ClearTextAsync(ILocator locator, string description = "")
        {
            Log.Information("Clear text — {Target}", Label(locator, description));
            await locator.ClearAsync();
        }

        /// <summary>Return the visible inner text of the element.</summary>
        protected async Task<string> GetTextAsync(ILocator locator, string description = "")
        {
            var text = await locator.InnerTextAsync();
            Log.Information("Get text '{Text}' — {Target}", text, Label(locator, description));
            return text;
        }

        /// <summary>Return the current value of an input / textarea.</summary>
        protected async Task<string> GetInputValueAsync(ILocator locator, string description = "")
        {
            var value = await locator.InputValueAsync();
            Log.Information("Get input value '{Value}' — {Target}", value, Label(locator, description));
            return value;
        }

        // -------------------------------------------------------------------------
        // Dropdown / Select actions
        // -------------------------------------------------------------------------

        /// <summary>Select a &lt;select&gt; option by its visible label.</summary>
        protected async Task SelectByLabelAsync(ILocator locator, string label, string description = "")
        {
            Log.Information("Select by label '{Label}' — {Target}", label, Label(locator, description));
            await locator.SelectOptionAsync(new SelectOptionValue { Label = label });
        }

        /// <summary>Select a &lt;select&gt; option by its value attribute.</summary>
        protected async Task SelectByValueAsync(ILocator locator, string value, string description = "")
        {
            Log.Information("Select by value '{Value}' — {Target}", value, Label(locator, description));
            await locator.SelectOptionAsync(new SelectOptionValue { Value = value });
        }

        /// <summary>Select a &lt;select&gt; option by its zero-based index.</summary>
        protected async Task SelectByIndexAsync(ILocator locator, int index, string description = "")
        {
            Log.Information("Select by index '{Index}' — {Target}", index, Label(locator, description));
            await locator.SelectOptionAsync(new SelectOptionValue { Index = index });
        }

        // -------------------------------------------------------------------------
        // Drag and drop
        // -------------------------------------------------------------------------

        /// <summary>Drag <paramref name="source"/> and drop it onto <paramref name="target"/>.</summary>
        protected async Task DragAndDropAsync(ILocator source, ILocator target, string description = "")
        {
            Log.Information("Drag & drop — {Description}", string.IsNullOrEmpty(description) ? $"{source} → {target}" : description);
            await source.DragToAsync(target);
        }

        // -------------------------------------------------------------------------
        // Keyboard actions
        // -------------------------------------------------------------------------

        /// <summary>
        /// Press a key while the element has focus.
        /// Accepts single keys ("Enter", "Tab") or chords ("Control+A").
        /// </summary>
        protected async Task PressKeyAsync(ILocator locator, string key, string description = "")
        {
            Log.Information("Press key '{Key}' — {Target}", key, Label(locator, description));
            await locator.PressAsync(key);
        }

        /// <summary>
        /// Send a keyboard shortcut at page level without targeting a specific element.
        /// Examples: "Control+C", "Shift+Tab", "Escape".
        /// </summary>
        protected async Task PressShortcutAsync(string key)
        {
            Log.Information("Press shortcut '{Key}'", key);
            await Page.Keyboard.PressAsync(key);
        }

        // -------------------------------------------------------------------------
        // Hover / Scroll
        // -------------------------------------------------------------------------

        /// <summary>Move the mouse pointer over the element.</summary>
        protected async Task HoverAsync(ILocator locator, string description = "")
        {
            Log.Information("Hover — {Target}", Label(locator, description));
            await locator.HoverAsync();
        }

        /// <summary>Scroll the element into the viewport.</summary>
        protected async Task ScrollIntoViewAsync(ILocator locator, string description = "")
        {
            Log.Information("Scroll into view — {Target}", Label(locator, description));
            await locator.ScrollIntoViewIfNeededAsync();
        }

        // -------------------------------------------------------------------------
        // State checks
        // -------------------------------------------------------------------------

        /// <summary>Returns true if the element is visible in the DOM.</summary>
        protected async Task<bool> IsVisibleAsync(ILocator locator) =>
            await locator.IsVisibleAsync();

        /// <summary>Returns true if the element is enabled (not disabled).</summary>
        protected async Task<bool> IsEnabledAsync(ILocator locator) =>
            await locator.IsEnabledAsync();

        /// <summary>Returns true if a checkbox or radio button is checked.</summary>
        protected async Task<bool> IsCheckedAsync(ILocator locator) =>
            await locator.IsCheckedAsync();

        // -------------------------------------------------------------------------
        // Wait
        // -------------------------------------------------------------------------

        /// <summary>Wait until the element satisfies the given state (default: Visible).</summary>
        protected async Task WaitForAsync(ILocator locator, LocatorWaitForOptions? options = null)
        {
            Log.Debug("Waiting for element: {Locator}", locator);
            await locator.WaitForAsync(options);
        }

        // -------------------------------------------------------------------------
        // Private helpers
        // -------------------------------------------------------------------------

        private static string Label(ILocator locator, string description) =>
            string.IsNullOrEmpty(description) ? locator.ToString()! : description;
    }
}
