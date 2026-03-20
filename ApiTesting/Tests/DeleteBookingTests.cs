using ApiTesting.Helpers;

namespace ApiTesting.Tests;

[TestFixture]
public class DeleteBookingTests
{
    private ApiClient _api = null!;
    private string _token = null!;

    [SetUp]
    public async Task SetUp()
    {
        _api = new ApiClient();
        await _api.InitializeAsync();
        _token = await _api.GetAuthTokenAsync();
    }

    [TearDown]
    public async Task TearDown() => await _api.DisposeAsync();

    [Test]
    public async Task DeleteBooking_WithTokenAuth_ShouldReturn201()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();

        var response = await _api.Request.DeleteAsync($"/booking/{bookingId}", new()
        {
            Headers = new Dictionary<string, string>
            {
                ["Cookie"] = $"token={_token}"
            }
        });

        Assert.That(response.Status, Is.EqualTo(201));
    }

    [Test]
    public async Task DeleteBooking_WithBasicAuth_ShouldReturn201()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();
        var credentials = Convert.ToBase64String("admin:password123"u8);

        var response = await _api.Request.DeleteAsync($"/booking/{bookingId}", new()
        {
            Headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Basic {credentials}"
            }
        });

        Assert.That(response.Status, Is.EqualTo(201));
    }

    [Test]
    public async Task DeleteBooking_ThenGet_ShouldReturn404()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();

        await _api.Request.DeleteAsync($"/booking/{bookingId}", new()
        {
            Headers = new Dictionary<string, string>
            {
                ["Cookie"] = $"token={_token}"
            }
        });

        var getResponse = await _api.Request.GetAsync($"/booking/{bookingId}");

        Assert.That(getResponse.Status, Is.EqualTo(404));
    }

    [Test]
    public async Task DeleteBooking_WithoutAuth_ShouldReturn403()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();

        var response = await _api.Request.DeleteAsync($"/booking/{bookingId}");

        Assert.That(response.Status, Is.EqualTo(403));
    }

    [Test]
    public async Task DeleteBooking_WithInvalidToken_ShouldReturn403()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();

        var response = await _api.Request.DeleteAsync($"/booking/{bookingId}", new()
        {
            Headers = new Dictionary<string, string>
            {
                ["Cookie"] = "token=invalidtoken123"
            }
        });

        Assert.That(response.Status, Is.EqualTo(403));
    }

    [Test]
    public async Task DeleteBooking_WithNonExistentId_ShouldReturn405()
    {
        var response = await _api.Request.DeleteAsync("/booking/999999999", new()
        {
            Headers = new Dictionary<string, string>
            {
                ["Cookie"] = $"token={_token}"
            }
        });

        // The API returns 405 for non-existent booking deletion
        Assert.That(response.Status, Is.AnyOf(404, 405));
    }

    [Test]
    public async Task DeleteBooking_ThenDeleteAgain_ShouldNotReturn201()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();

        await _api.Request.DeleteAsync($"/booking/{bookingId}", new()
        {
            Headers = new Dictionary<string, string>
            {
                ["Cookie"] = $"token={_token}"
            }
        });

        var secondDelete = await _api.Request.DeleteAsync($"/booking/{bookingId}", new()
        {
            Headers = new Dictionary<string, string>
            {
                ["Cookie"] = $"token={_token}"
            }
        });

        Assert.That(secondDelete.Status, Is.Not.EqualTo(201),
            "Deleting an already-deleted booking should not succeed");
    }
}
