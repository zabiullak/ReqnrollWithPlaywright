using ApiTesting.Models;

namespace ApiTesting.Helpers;

public static class BookingDataFactory
{
    public static BookingPayload CreateDefaultBooking() => new()
    {
        Firstname = "James",
        Lastname = "Brown",
        Totalprice = 111,
        Depositpaid = true,
        Bookingdates = new BookingDates
        {
            Checkin = "2025-01-01",
            Checkout = "2025-01-10"
        },
        Additionalneeds = "Breakfast"
    };

    public static object CreateDefaultBookingAsAnonymous() => new
    {
        firstname = "James",
        lastname = "Brown",
        totalprice = 111,
        depositpaid = true,
        bookingdates = new
        {
            checkin = "2025-01-01",
            checkout = "2025-01-10"
        },
        additionalneeds = "Breakfast"
    };

    public static object CreateUpdatedBookingAsAnonymous() => new
    {
        firstname = "Jane",
        lastname = "Doe",
        totalprice = 250,
        depositpaid = false,
        bookingdates = new
        {
            checkin = "2025-06-01",
            checkout = "2025-06-15"
        },
        additionalneeds = "Lunch"
    };

    public static object CreatePartialUpdate() => new
    {
        firstname = "UpdatedFirstName",
        lastname = "UpdatedLastName"
    };
}
