using System;

namespace MyUALife
{
    [Serializable()]
    public class Deadline : IComparable<Deadline>
    {
        /*
         * Constructs a new Deadline with the given properties.
         */
        public Deadline(String name, String desc, DateTime time, EventType eventType)
        {
            Name = name;
            Description = desc;
            Time = time;
            Type = eventType;
        }

        /*
         * Constructs a new Deadline with the given properties and a default
         * EventType.
         */
        public Deadline(String name, String desc, DateTime time) : 
            this(name, desc, time, Category.other) { }

        /*
         * Constructor for serialization.
         */
        private Deadline() { }

        // Simple properties
        public DateTime  Time        { get; set; }
        public String    Name        { get; set; }
        public String    Description { get; set; }
        public EventType Type        { get; set; }

        /*
         * Returns a human-readable String representation of the Deadline.
         */
        public override String ToString()
        {
            String desc = "";
            if (Description != "")
            {
                desc = "Description: " + Description + "\n";
            }
            String format = "Name: {0}\n{1}Time: {2}\nType: {3}";
            Object[] args = { Name, desc, Time, Type.Name };
            return String.Format(format, args);
        }

        /*
         * Compares two Deadlines on the basis of their times.
         */
        public int CompareTo(Deadline d)
        {
            return Time.CompareTo(d.Time);
        }
    }

}