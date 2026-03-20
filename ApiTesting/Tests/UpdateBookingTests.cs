using ApiTesting.Helpers;

namespace ApiTesting.Tests;

[TestFixture]
public class UpdateBookingTests
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
    public async Task UpdateBooking_WithTokenAuth_ShouldReturn200WithUpdatedData()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();
        var updatedPayload = BookingDataFactory.CreateUpdatedBookingAsAnonymous();

        var response = await _api.Request.PutAsync($"/booking/{bookingId}", new()
        {
            DataObject = updatedPayload,
            Headers = new Dictionary<string, string>
            {
                ["Cookie"] = $"token={_token}"
            }
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();

        Assert.Multiple(() =>
        {
            Assert.That(json.Value.GetProperty("firstname").GetString(), Is.EqualTo("Jane"));
            Assert.That(json.Value.GetProperty("lastname").GetString(), Is.EqualTo("Doe"));
            Assert.That(json.Value.GetProperty("totalprice").GetInt32(), Is.EqualTo(250));
            Assert.That(json.Value.GetProperty("depositpaid").GetBoolean(), Is.False);
            Assert.That(json.Value.GetProperty("bookingdates").GetProperty("checkin").GetString(), Is.EqualTo("2025-06-01"));
            Assert.That(json.Value.GetProperty("bookingdates").GetProperty("checkout").GetString(), Is.EqualTo("2025-06-15"));
            Assert.That(json.Value.GetProperty("additionalneeds").GetString(), Is.EqualTo("Lunch"));
        });
    }

    [Test]
    public async Task UpdateBooking_WithBasicAuth_ShouldReturn200()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();
        var updatedPayload = BookingDataFactory.CreateUpdatedBookingAsAnonymous();

        var credentials = Convert.ToBase64String("admin:password123"u8);

        var response = await _api.Request.PutAsync($"/booking/{bookingId}", new()
        {
            DataObject = updatedPayload,
            Headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Basic {credentials}"
            }
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();
        Assert.That(json.Value.GetProperty("firstname").GetString(), Is.EqualTo("Jane"));
    }

    [Test]
    public async Task UpdateBooking_WithoutAuth_ShouldReturn403()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();
        var updatedPayload = BookingDataFactory.CreateUpdatedBookingAsAnonymous();

        var response = await _api.Request.PutAsync($"/booking/{bookingId}", new()
        {
            DataObject = updatedPayload
        });

        Assert.That(response.Status, Is.EqualTo(403));
    }

    [Test]
    public async Task UpdateBooking_WithInvalidToken_ShouldReturn403()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();
        var updatedPayload = BookingDataFactory.CreateUpdatedBookingAsAnonymous();

        var response = await _api.Request.PutAsync($"/booking/{bookingId}", new()
        {
            DataObject = updatedPayload,
            Headers = new Dictionary<string, string>
            {
                ["Cookie"] = "token=invalidtoken123"
            }
        });

        Assert.That(response.Status, Is.EqualTo(403));
    }

    [Test]
    public async Task UpdateBooking_VerifyGetReflectsUpdate_ShouldReturnUpdatedData()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();

        await _api.Request.PutAsync($"/booking/{bookingId}", new()
        {
            DataObject = BookingDataFactory.CreateUpdatedBookingAsAnonymous(),
            Headers = new Dictionary<string, string>
            {
                ["Cookie"] = $"token={_token}"
            }
        });

        var getResponse = await _api.Request.GetAsync($"/booking/{bookingId}");

        Assert.That(getResponse.Status, Is.EqualTo(200));

        var json = await getResponse.JsonAsync();

        Assert.Multiple(() =>
        {
            Assert.That(json.Value.GetProperty("firstname").GetString(), Is.EqualTo("Jane"));
            Assert.That(json.Value.GetProperty("lastname").GetString(), Is.EqualTo("Doe"));
        });
    }
}
