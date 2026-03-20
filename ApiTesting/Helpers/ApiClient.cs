using Microsoft.Playwright;

namespace ApiTesting.Helpers;

public class ApiClient : IAsyncDisposable
{
    private const string BaseUrl = "https://restful-booker.herokuapp.com";

    private IPlaywright _playwright = null!;
    public IAPIRequestContext Request { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        Request = await _playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = BaseUrl,
            IgnoreHTTPSErrors = true,
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Accept"] = "application/json"
            }
        });
    }

    public async Task<string> GetAuthTokenAsync(
        string username = "admin",
        string password = "password123")
    {
        var response = await Request.PostAsync("/auth", new()
        {
            DataObject = new { username, password }
        });
        var json = await response.JsonAsync();
        return json.Value.GetProperty("token").GetString()!;
    }

    public async Task<int> CreateBookingAndGetIdAsync()
    {
        var response = await Request.PostAsync("/booking", new()
        {
            DataObject = BookingDataFactory.CreateDefaultBookingAsAnonymous()
        });
        var json = await response.JsonAsync();
        return json.Value.GetProperty("bookingid").GetInt32();
    }

    public async ValueTask DisposeAsync()
    {
        if (Request is not null)
            await Request.DisposeAsync();

        _playwright?.Dispose();
    }
}
