using System;
namespace MyUALife
{
    public class Model
    {
        public static Calendar Calendar
        {
            get;
        }

        static Model()
        {
            Calendar = new Calendar();

            var type = Category.recreation;

            DateTime now = DateTime.Now;
            Calendar.AddEvent(new Event("First Event", "", type, now, now));
            Calendar.AddEvent(new Event("Second Event", "", type, now, now));
            Calendar.AddEvent(new Event("Third Event", "", type, now, now));

            DateTime midnightMorning = DateTime.Today;
            Calendar.AddEvent(new Event("Last Midnight", "", type, midnightMorning, midnightMorning));

            DateTime midnightNight = DateTime.Today.AddDays(1);
            Calendar.AddEvent(new Event("Midnight Tonight", "", type, midnightNight, midnightNight));
        }
    }
}