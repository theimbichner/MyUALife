using System;
namespace MyUALife
{
    public class Model
    {
        private static readonly Calendar calendar = new Calendar();

        static Model()
        {
            DateTime now = DateTime.Now;
            calendar.AddEvent(new Event("First Event", now, now));
            calendar.AddEvent(new Event("Second Event", now, now));
            calendar.AddEvent(new Event("Third Event", now, now));

            DateTime midnightMorning = DateTime.Today;
            calendar.AddEvent(new Event("Last Midnight", midnightMorning, midnightMorning));

            DateTime midnightNight = DateTime.Today.AddDays(1);
            calendar.AddEvent(new Event("Midnight Tonight", midnightNight, midnightNight));
        }

        public static Calendar getCalendar()
        {
            return calendar;
        }
    }
}