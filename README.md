# ReqnrollWithPlaywright

A BDD UI test automation framework built with **Reqnroll**, **Microsoft Playwright**, and **NUnit** targeting **.NET 10**.  
Tests cover the [BBC Weather](https://www.bbc.com/weather) website and generate rich HTML reports via **Allure**.

---

## Tech Stack

| Tool / Library | Version | Purpose |
|---|---|---|
| .NET | 10 | Target framework |
| Reqnroll.NUnit | 3.2.0 | BDD (Gherkin) test runner |
| Microsoft.Playwright | 1.58.0 | Browser automation |
| NUnit | 4.4.0 | Test framework |
| NUnit3TestAdapter | 5.1.0 | Visual Studio / CLI test discovery |
| Allure.Reqnroll | 2.14.1 | HTML test reporting |
| FluentAssertions | 8.8.0 | Readable assertions |
| Serilog | 4.3.1 | Structured logging (console + file) |

---

## Project Structure

```
ReqnrollWithPlaywright/
│
├── Drivers/
│   └── PlaywrightDriver.cs       # Browser lifecycle (launch, trace, screenshot, dispose)
│
├── Features/
│   ├── Weather1.feature          # Scenarios: weather search + News page
│   └── Weather2.feature
│
├── Hooks/
│   ├── Hook.cs                   # BeforeScenario / AfterStep / AfterScenario
│   └── LoggerSetup.cs            # Serilog initialisation & teardown
│
├── Pages/
│   ├── BasePage.cs               # Reusable UI action methods (see below)
│   ├── HomePage.cs               # BBC Weather home page
│   └── NewsPage.cs               # BBC News page
│
├── StepDefinitions/
│   └── Weather.cs                # Gherkin step bindings
│
├── Support/
│   ├── Dependencies.cs           # Reqnroll DI container setup
│   ├── Helpers.cs                # Date/time utilities
│   └── RunSettingsReader.cs      # Thread-safe runsettings reader
│
├── Settings.runsettings          # Runtime configuration (browser, headless, URL …)
├── allureConfig.json             # Allure results output path
└── ReqnrollWithPlaywright.csproj
│
RunTests.ps1                      # One-click: run tests + generate Allure HTML report
```

---

## Prerequisites

1. [.NET 10 SDK](https://dotnet.microsoft.com/download)
2. Playwright browsers installed:
   ```powershell
   pwsh ReqnrollWithPlaywright\bin\Debug\net10.0\playwright.ps1 install
   ```
3. Allure CLI (for HTML reports):
   ```powershell
   npm install -g allure-commandline
   ```

---

## Configuration

All runtime settings live in `ReqnrollWithPlaywright/Settings.runsettings`:

```xml
<TestRunParameters>
    <Parameter name="Browser"   value="Chromium" />
    <Parameter name="Headless"  value="false" />
    <Parameter name="BaseUrl"   value="https://www.bbc.com/weather" />
    <Parameter name="Username"  value="" />
    <Parameter name="Password"  value="" />
</TestRunParameters>
```

Set `Headless` to `true` for CI/headless environments.

---

## Running Tests

### Option 1 — One-click (tests + Allure report)

```powershell
.\RunTests.ps1
```

This script:
1. Cleans previous `allure-results/` and `TestReport/`
2. Runs `dotnet test`
3. Generates the HTML report into `TestReport/`
4. Opens the report in your browser via `allure open`

### Option 2 — `dotnet test` directly

```powershell
# Run all tests
dotnet test

# Run by a single tag
dotnet test --filter "TestCategory=LocalWeather"

# Run a specific scenario
dotnet test --filter "TestCategory=Test-001"

# AND — must match both tags
dotnet test --filter "TestCategory=LocalWeather&TestCategory=Test-001"

# OR — match either tag
dotnet test --filter "TestCategory=Test-001|TestCategory=Test-003"

# NOT — exclude a tag
dotnet test --filter "TestCategory!=Test-003"
```

---

## Test Scenarios

### `Weather1.feature` / `Weather2.feature`

| Tag | Scenario | Description |
|---|---|---|
| `@LocalWeather` `@Test-001` | Check My Local Weather | Search for Bangalore weather, assert location heading |
| `@LocalWeather` `@Test-002` | Check My Local Weather | Search for Chennai Airport weather, assert location heading |
| `@Test-003` | Check News page | Click News link, assert page title contains "News" |

---

## Reporting

After each run the Allure HTML report is generated at:

```
TestReport/
```

To view it manually without `RunTests.ps1`:

```powershell
allure generate allure-results --clean -o TestReport
allure open TestReport
```

**On failure**, the report automatically includes:
- 📸 Full-page **screenshot** embedded as an attachment
- 🎥 Playwright **trace** file saved to `Traces/` (viewable with `playwright show-trace`)

```powershell
# View a saved trace
pwsh playwright.ps1 show-trace "Traces\<scenario>_<timestamp>.zip"
```

---

## Logging

[Serilog](https://serilog.net/) writes structured logs to:

| Sink | Location | Level |
|---|---|---|
| Console | Terminal | Debug+ |
| Rolling file | `Logs/test-log-<date>.txt` | Debug+ |

Logs capture every navigation, click, input, assertion pass/fail, and scenario start/end.

---

## Architecture

### Page Object Model

All page classes extend `BasePage`, which provides ready-to-use, logged UI action methods:

```csharp
// Click
await ClickAsync(SubmitButton, "Submit button");

// Text input
await EnterTextAsync(SearchBox, "Bangalore", "Search box");
await TypeTextAsync(OtpField, "123456", "OTP field");       // keystroke-by-keystroke

// Dropdowns
await SelectByLabelAsync(CountryDropdown, "India", "Country");
await SelectByValueAsync(CountryDropdown, "IN");
await SelectByIndexAsync(CountryDropdown, 0);

// Keyboard
await PressKeyAsync(SearchBox, "Enter");
await PressShortcutAsync("Control+A");

// Drag & drop
await DragAndDropAsync(DraggableCard, DropZone);

// Read
string heading = await GetTextAsync(PageHeading, "Page heading");
string value   = await GetInputValueAsync(InputField);

// State
bool visible = await IsVisibleAsync(Banner);
bool enabled = await IsEnabledAsync(SaveButton);
bool checked_ = await IsCheckedAsync(AgreeCheckbox);

// Hover / scroll
await HoverAsync(Tooltip);
await ScrollIntoViewAsync(FooterLink);

// Wait
await WaitForAsync(Modal, new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
```

### Hooks lifecycle

```
[BeforeTestRun]   → Serilog initialised
[BeforeScenario]  → Playwright browser + page + tracing started
[AfterStep]       → On failure: trace saved, screenshot captured, attached to Allure
[AfterScenario]   → Browser closed, resources disposed
[AfterTestRun]    → Serilog flushed
```

---

## Output Folders

| Folder | Contents | Git tracked |
|---|---|---|
| `Logs/` | Rolling daily log files | ❌ |
| `Screenshots/` | Failure screenshots (`.png`) | ❌ |
| `Traces/` | Playwright trace archives (`.zip`) | ❌ |
| `allure-results/` | Raw Allure result files | ❌ |
| `TestReport/` | Generated HTML report | ❌ |

---

## Roadmap

- [ ] API testing layer
- [ ] Retry failed tests
- [ ] CI/CD pipeline integration (GitHub Actions)
