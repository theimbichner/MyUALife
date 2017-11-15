using System;

namespace MyUALife
{
    [Serializable()]
    public class Event : IComparable<Event>
    {
        /*
         * Constructs a new event with the given propeties.
         */
        public Event(String name, String desc, EventType type, DateTime start, DateTime end)
        {
            if (start >= end)
            {
                throw new ArgumentException();
            }

            Name = name;
            Description = desc;
            Type = type;
            StartTime = start;
            EndTime = end;
        }

        /*
         * Constructor for serialization.
         */
        private Event() { }

        // Simple properties
        public DateTime  StartTime   { get; set; }
        public DateTime  EndTime     { get; set; }
        public String    Name        { get; set; }
        public String    Description { get; set; }
        public EventType Type        { get; set; }

        /*
         * Returns a new event whose properties are identical to this one. However,
         * the new event begins and ends shift time in the future.
         */
        public Event Shift(TimeSpan shift)
        {
            return new Event(Name, Description, Type, StartTime + shift, EndTime + shift);
        }

        /*
         * Returns a human-readable String representation of the Event.
         */
        public override String ToString()
        {
            String desc = "";
            if (Description != "")
            {
                desc = "Description: " + Description + "\n";
            }
            String format = "Name: {0}\n{1}From: {2}\nTo: {3}\nType: {4}";
            Object[] args = { Name, desc, StartTime, EndTime, Type.Name };
            return String.Format(format, args);
        }

        /*
         * Compares two Events on the basis of their start times.
         */
        public int CompareTo(Event e)
        {
            return StartTime.CompareTo(e.StartTime);
        }
    }
}