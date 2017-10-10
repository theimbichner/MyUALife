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
        }

        public static Calendar getCalendar()
        {
            return calendar;
        }
    }
}