
namespace ReqnrollWithPlaywright.Support
{
    public static class Helper
    {
        public static DateTime GetDateValue(int addDays)
        {
            DateTime timeUtc = DateTime.UtcNow;
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
            return cstTime.AddDays(addDays);
        }
    }
}
