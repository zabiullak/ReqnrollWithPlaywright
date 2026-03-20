namespace ApiTesting.Models;

public class BookingResponse
{
    public int Bookingid { get; set; }
    public BookingPayload Booking { get; set; } = new();
}
