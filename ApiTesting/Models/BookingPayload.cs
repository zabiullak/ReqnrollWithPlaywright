namespace ApiTesting.Models;

public class BookingPayload
{
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public int Totalprice { get; set; }
    public bool Depositpaid { get; set; }
    public BookingDates Bookingdates { get; set; } = new();
    public string Additionalneeds { get; set; } = string.Empty;
}
