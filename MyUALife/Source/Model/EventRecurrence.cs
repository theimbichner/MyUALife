using System;

namespace MyUALife
{
    class EventRecurrence
    {
        private DateTime updatedThrough;
        private Event baseEvent;
        private TimeSpan spacing;

        public EventRecurrence(Event baseEvent, TimeSpan spacing)
        {
            updatedThrough = baseEvent.EndTime;
            this.baseEvent = baseEvent;
            this.spacing = spacing;
        }

        public void Update(Calendar c, DateTime endTime)
        {

        }
    }
}