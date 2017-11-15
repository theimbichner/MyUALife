using System;

namespace MyUALife
{
    [Serializable()]
    public struct EventType
    {
        /*
         * Constructs an EventType with the given properties.
         */
        public EventType(String name, int priority, bool isEditable, String color)
        {
            Name = name;
            Priority = priority;
            IsEditable = isEditable;
            ColorString = color;
        }

        // Simple properties
        public String Name        { get; }
        public bool   IsEditable  { get; }
        public int    Priority    { get; }
        public String ColorString { get; }
    }
}