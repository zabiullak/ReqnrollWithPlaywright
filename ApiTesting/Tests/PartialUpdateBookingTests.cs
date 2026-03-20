using ApiTesting.Helpers;

namespace ApiTesting.Tests;

[TestFixture]
public class PartialUpdateBookingTests
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
    public async Task PartialUpdate_FirstAndLastName_ShouldReturn200WithUpdatedFields()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();

        var response = await _api.Request.PatchAsync($"/booking/{bookingId}", new()
        {
            DataObject = new { firstname = "UpdatedFirst", lastname = "UpdatedLast" },
            Headers = new Dictionary<string, string>
            {
                ["Cookie"] = $"token={_token}"
            }
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();

        Assert.Multiple(() =>
        {
            Assert.That(json.Value.GetProperty("firstname").GetString(), Is.EqualTo("UpdatedFirst"));
            Assert.That(json.Value.GetProperty("lastname").GetString(), Is.EqualTo("UpdatedLast"));
            // Other fields should remain unchanged
            Assert.That(json.Value.GetProperty("totalprice").GetInt32(), Is.EqualTo(111));
            Assert.That(json.Value.GetProperty("depositpaid").GetBoolean(), Is.True);
        });
    }

    [Test]
    public async Task PartialUpdate_OnlyPrice_ShouldUpdatePriceOnly()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();

        var response = await _api.Request.PatchAsync($"/booking/{bookingId}", new()
        {
            DataObject = new { totalprice = 999 },
            Headers = new Dictionary<string, string>
            {
                ["Cookie"] = $"token={_token}"
            }
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();

        Assert.Multiple(() =>
        {
            Assert.That(json.Value.GetProperty("totalprice").GetInt32(), Is.EqualTo(999));
            Assert.That(json.Value.GetProperty("firstname").GetString(), Is.EqualTo("James"));
        });
    }

    [Test]
    public async Task PartialUpdate_OnlyDates_ShouldUpdateDatesOnly()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();

        var response = await _api.Request.PatchAsync($"/booking/{bookingId}", new()
        {
            DataObject = new
            {
                bookingdates = new
                {
                    checkin = "2025-12-01",
                    checkout = "2025-12-25"
                }
            },
            Headers = new Dictionary<string, string>
            {
                ["Cookie"] = $"token={_token}"
            }
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();
        var dates = json.Value.GetProperty("bookingdates");

        Assert.Multiple(() =>
        {
            Assert.That(dates.GetProperty("checkin").GetString(), Is.EqualTo("2025-12-01"));
            Assert.That(dates.GetProperty("checkout").GetString(), Is.EqualTo("2025-12-25"));
        });
    }

    [Test]
    public async Task PartialUpdate_WithBasicAuth_ShouldReturn200()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();
        var credentials = Convert.ToBase64String("admin:password123"u8);

        var response = await _api.Request.PatchAsync($"/booking/{bookingId}", new()
        {
            DataObject = new { firstname = "BasicAuthUser" },
            Headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Basic {credentials}"
            }
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();
        Assert.That(json.Value.GetProperty("firstname").GetString(), Is.EqualTo("BasicAuthUser"));
    }

    [Test]
    public async Task PartialUpdate_WithoutAuth_ShouldReturn403()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();

        var response = await _api.Request.PatchAsync($"/booking/{bookingId}", new()
        {
            DataObject = new { firstname = "NoAuth" }
        });

        Assert.That(response.Status, Is.EqualTo(403));
    }

    [Test]
    public async Task PartialUpdate_WithInvalidToken_ShouldReturn403()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();

        var response = await _api.Request.PatchAsync($"/booking/{bookingId}", new()
        {
            DataObject = new { firstname = "BadToken" },
            Headers = new Dictionary<string, string>
            {
                ["Cookie"] = "token=invalidtoken123"
            }
        });

        Assert.That(response.Status, Is.EqualTo(403));
    }
}
