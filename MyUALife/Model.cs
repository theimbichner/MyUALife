namespace MyUALife
{
    public class Model
    {
        private static readonly Calendar calendar = new Calendar();

        public static Calendar getCalendar()
        {
            return calendar;
        }
    }
}