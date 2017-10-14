using System;
namespace MyUALife
{
    public class Model
    {
        private static readonly Calendar calendar = new Calendar();

        static Model()
        {
            var type = Category.recreation;

            DateTime now = DateTime.Now;
            calendar.AddEvent(new Event("First Event", "", type, now, now));
            calendar.AddEvent(new Event("Second Event", "", type, now, now));
            calendar.AddEvent(new Event("Third Event", "", type, now, now));

            DateTime midnightMorning = DateTime.Today;
            calendar.AddEvent(new Event("Last Midnight", "", type, midnightMorning, midnightMorning));

            DateTime midnightNight = DateTime.Today.AddDays(1);
            calendar.AddEvent(new Event("Midnight Tonight", "", type, midnightNight, midnightNight));
        }

        public static Calendar getCalendar()
        {
            return calendar;
        }
    }
}