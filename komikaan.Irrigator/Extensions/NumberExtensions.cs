namespace komikaan.Irrigator.Extensions
{
    public static class NumberExtensions
    {

        public static DateTimeOffset ToDateTime(this long seconds)
        {
            return ToDateTime((long?) seconds)!.Value;
        }
        public static DateTimeOffset? ToDateTime(this ulong seconds)
        {
            return ToDateTime((long?)seconds);
        }

        public static DateTimeOffset? ToDateTime(this ulong? seconds)
        {
            return ToDateTime((long?)seconds);
        }

        public static DateTimeOffset? ToDateTime(this long? seconds)
        {
            if (seconds != null)
            {
                DateTimeOffset dateTime = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.FromMicroseconds(0));
                dateTime = dateTime.AddSeconds((long)seconds).ToLocalTime();
                return dateTime;
            }
            return null;
        }
    }
}
