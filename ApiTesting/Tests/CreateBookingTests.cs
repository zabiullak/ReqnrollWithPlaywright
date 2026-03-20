using ApiTesting.Helpers;

namespace ApiTesting.Tests;

[TestFixture]
public class CreateBookingTests
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
    public async Task CreateBooking_WithValidData_ShouldReturn200WithBookingDetails()
    {
        var payload = BookingDataFactory.CreateDefaultBookingAsAnonymous();

        var response = await _api.Request.PostAsync("/booking", new()
        {
            DataObject = payload
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();
        var bookingId = json.Value.GetProperty("bookingid").GetInt32();
        var booking = json.Value.GetProperty("booking");

        Assert.Multiple(() =>
        {
            Assert.That(bookingId, Is.GreaterThan(0));
            Assert.That(booking.GetProperty("firstname").GetString(), Is.EqualTo("James"));
            Assert.That(booking.GetProperty("lastname").GetString(), Is.EqualTo("Brown"));
            Assert.That(booking.GetProperty("totalprice").GetInt32(), Is.EqualTo(111));
            Assert.That(booking.GetProperty("depositpaid").GetBoolean(), Is.True);
            Assert.That(booking.GetProperty("bookingdates").GetProperty("checkin").GetString(), Is.EqualTo("2025-01-01"));
            Assert.That(booking.GetProperty("bookingdates").GetProperty("checkout").GetString(), Is.EqualTo("2025-01-10"));
            Assert.That(booking.GetProperty("additionalneeds").GetString(), Is.EqualTo("Breakfast"));
        });
    }

    [Test]
    public async Task CreateBooking_WithoutAdditionalNeeds_ShouldSucceed()
    {
        var payload = new
        {
            firstname = "NoExtras",
            lastname = "Guest",
            totalprice = 50,
            depositpaid = false,
            bookingdates = new
            {
                checkin = "2025-03-01",
                checkout = "2025-03-05"
            }
        };

        var response = await _api.Request.PostAsync("/booking", new()
        {
            DataObject = payload
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();
        Assert.That(json.Value.GetProperty("bookingid").GetInt32(), Is.GreaterThan(0));
    }

    [Test]
    public async Task CreateBooking_WithZeroPrice_ShouldSucceed()
    {
        var payload = new
        {
            firstname = "Free",
            lastname = "Stay",
            totalprice = 0,
            depositpaid = true,
            bookingdates = new
            {
                checkin = "2025-02-01",
                checkout = "2025-02-02"
            },
            additionalneeds = "None"
        };

        var response = await _api.Request.PostAsync("/booking", new()
        {
            DataObject = payload
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();
        var booking = json.Value.GetProperty("booking");

        Assert.That(booking.GetProperty("totalprice").GetInt32(), Is.EqualTo(0));
    }

    [Test]
    public async Task CreateBooking_WithDepositNotPaid_ShouldReflectInResponse()
    {
        var payload = new
        {
            firstname = "No",
            lastname = "Deposit",
            totalprice = 200,
            depositpaid = false,
            bookingdates = new
            {
                checkin = "2025-04-01",
                checkout = "2025-04-10"
            }
        };

        var response = await _api.Request.PostAsync("/booking", new()
        {
            DataObject = payload
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();
        var depositPaid = json.Value.GetProperty("booking").GetProperty("depositpaid").GetBoolean();

        Assert.That(depositPaid, Is.False);
    }

    [Test]
    public async Task CreateBooking_WithSpecialCharactersInName_ShouldSucceed()
    {
        var payload = new
        {
            firstname = "José",
            lastname = "O'Brien",
            totalprice = 100,
            depositpaid = true,
            bookingdates = new
            {
                checkin = "2025-05-01",
                checkout = "2025-05-05"
            }
        };

        var response = await _api.Request.PostAsync("/booking", new()
        {
            DataObject = payload
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();
        var booking = json.Value.GetProperty("booking");

        Assert.Multiple(() =>
        {
            Assert.That(booking.GetProperty("firstname").GetString(), Is.EqualTo("José"));
            Assert.That(booking.GetProperty("lastname").GetString(), Is.EqualTo("O'Brien"));
        });
    }
}
