using System;

namespace MyUALife
{
    public class EventRecurrence
    {
        private DateTime updatedThrough; // Events who start before or on this time are in the calendar
        private readonly Event baseEvent; // The first event -- we don't care about events before this
        private readonly TimeSpan spacing;

        public EventRecurrence(Event baseEvent, TimeSpan spacing)
        {
            updatedThrough = baseEvent.StartTime.AddMinutes(-1);
            this.baseEvent = baseEvent;
            this.spacing = spacing;
        }

        public void Update(Calendar calendar, DateTime endTime)
        {
            if (endTime <= updatedThrough)
            {
                throw new ArgumentException();
            }

            Event calendarEvent = baseEvent;

            // Get the first event that is not yet added
            while (calendarEvent.StartTime <= updatedThrough)
            {
                calendarEvent = calendarEvent.Shift(spacing);
            }

            // Add all the necessary new events
            while (calendarEvent.StartTime <= endTime)
            {
                calendar.AddEvent(calendarEvent);
                calendarEvent = calendarEvent.Shift(spacing);
            }
            updatedThrough = endTime;
        }
    }
}