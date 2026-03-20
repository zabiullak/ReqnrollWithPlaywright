using ApiTesting.Helpers;

namespace ApiTesting.Tests;

[TestFixture]
public class GetBookingTests
{
    private ApiClient _api = null!;

    [SetUp]
    public async Task SetUp()
    {
        _api = new ApiClient();
        await _api.InitializeAsync();
    }

    [TearDown]
    public async Task TearDown() => await _api.DisposeAsync();

    [Test]
    public async Task GetBookingIds_ShouldReturnNonEmptyList()
    {
        var response = await _api.Request.GetAsync("/booking");

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();
        var bookings = json.Value.EnumerateArray();

        Assert.That(bookings.Any(), Is.True, "Booking list should not be empty");
    }

    [Test]
    public async Task GetBookingIds_FilterByFirstName_ShouldReturnFilteredResults()
    {
        // Create a booking to ensure the filter has data
        await _api.Request.PostAsync("/booking", new()
        {
            DataObject = BookingDataFactory.CreateDefaultBookingAsAnonymous()
        });

        var response = await _api.Request.GetAsync("/booking", new()
        {
            Params = new Dictionary<string, object> { ["firstname"] = "James" }
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();
        Assert.That(json.Value.EnumerateArray().Any(), Is.True);
    }

    [Test]
    public async Task GetBookingIds_FilterByLastName_ShouldReturnFilteredResults()
    {
        await _api.Request.PostAsync("/booking", new()
        {
            DataObject = BookingDataFactory.CreateDefaultBookingAsAnonymous()
        });

        var response = await _api.Request.GetAsync("/booking", new()
        {
            Params = new Dictionary<string, object> { ["lastname"] = "Brown" }
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();
        Assert.That(json.Value.EnumerateArray().Any(), Is.True);
    }

    [Test]
    public async Task GetBookingIds_FilterByCheckinDate_ShouldReturnResults()
    {
        await _api.Request.PostAsync("/booking", new()
        {
            DataObject = BookingDataFactory.CreateDefaultBookingAsAnonymous()
        });

        var response = await _api.Request.GetAsync("/booking", new()
        {
            Params = new Dictionary<string, object> { ["checkin"] = "2024-01-01" }
        });

        Assert.That(response.Status, Is.EqualTo(200));
    }

    [Test]
    public async Task GetBookingIds_FilterByCheckoutDate_ShouldReturnResults()
    {
        await _api.Request.PostAsync("/booking", new()
        {
            DataObject = BookingDataFactory.CreateDefaultBookingAsAnonymous()
        });

        var response = await _api.Request.GetAsync("/booking", new()
        {
            Params = new Dictionary<string, object> { ["checkout"] = "2026-12-31" }
        });

        Assert.That(response.Status, Is.EqualTo(200));
    }

    [Test]
    public async Task GetBookingIds_FilterByFirstAndLastName_ShouldReturnResults()
    {
        await _api.Request.PostAsync("/booking", new()
        {
            DataObject = BookingDataFactory.CreateDefaultBookingAsAnonymous()
        });

        var response = await _api.Request.GetAsync("/booking", new()
        {
            Params = new Dictionary<string, object>
            {
                ["firstname"] = "James",
                ["lastname"] = "Brown"
            }
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();
        Assert.That(json.Value.EnumerateArray().Any(), Is.True);
    }

    [Test]
    public async Task GetBookingById_WithValidId_ShouldReturnBookingDetails()
    {
        var bookingId = await _api.CreateBookingAndGetIdAsync();

        var response = await _api.Request.GetAsync($"/booking/{bookingId}");

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();

        Assert.Multiple(() =>
        {
            Assert.That(json.Value.GetProperty("firstname").GetString(), Is.EqualTo("James"));
            Assert.That(json.Value.GetProperty("lastname").GetString(), Is.EqualTo("Brown"));
            Assert.That(json.Value.GetProperty("totalprice").GetInt32(), Is.EqualTo(111));
            Assert.That(json.Value.GetProperty("depositpaid").GetBoolean(), Is.True);
            Assert.That(json.Value.GetProperty("bookingdates").GetProperty("checkin").GetString(), Is.EqualTo("2025-01-01"));
            Assert.That(json.Value.GetProperty("bookingdates").GetProperty("checkout").GetString(), Is.EqualTo("2025-01-10"));
            Assert.That(json.Value.GetProperty("additionalneeds").GetString(), Is.EqualTo("Breakfast"));
        });
    }

    [Test]
    public async Task GetBookingById_WithInvalidId_ShouldReturnNotFound()
    {
        var response = await _api.Request.GetAsync("/booking/999999999");

        Assert.That(response.Status, Is.EqualTo(404));
    }
}
