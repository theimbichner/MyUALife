using Android.Content;
using System;

namespace MyUALife
{
    [Serializable()]
    public class Event : IComparable<Event>
    {
        // Key fragments for specifying individual fields of events
        private const String ExtraEventName = ".EventName";
        private const String ExtraEventDescription = ".EventDescription";
        private const String ExtraEventStartTime = ".EventStartTime";
        private const String ExtraEventEndTime = ".EventEndTime";

        // Key fragments for specifying fields of the event type
        private const String ExtraEventTypeName = ".EventType.Name";
        private const String ExtraEventTypePriority = ".EventType.Priority";
        private const String ExtraEventTypeEditable = ".EventType.Editable";
        private const String ExtraEventTypeColor = ".EventType.Color";

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
         * Writes the given event to the intent using the supplied key.
         */
        public static void WriteEvent(Intent intent, String keyBase, Event calendarEvent)
        {
            // Write the event fields
            intent.PutExtra(keyBase + ExtraEventName, calendarEvent.Name);
            intent.PutExtra(keyBase + ExtraEventDescription, calendarEvent.Description);
            intent.PutExtra(keyBase + ExtraEventStartTime, calendarEvent.StartTime.ToBinary());
            intent.PutExtra(keyBase + ExtraEventEndTime, calendarEvent.EndTime.ToBinary());

            // Write the event type fields
            intent.PutExtra(keyBase + ExtraEventTypeName, calendarEvent.Type.Name);
            intent.PutExtra(keyBase + ExtraEventTypePriority, calendarEvent.Type.Priority);
            intent.PutExtra(keyBase + ExtraEventTypeEditable, calendarEvent.Type.IsEditable);
            intent.PutExtra(keyBase + ExtraEventTypeColor, calendarEvent.Type.ColorString);
        }

        /*
         * Reads the event associated with the given key from the intent.
         */
        public static Event ReadEvent(Intent intent, String keyBase)
        {
            // Return null in the case that there is no event
            if (!intent.HasExtra(keyBase + ExtraEventName))
            {
                return null;
            }

            // Read the event fields
            String name = intent.GetStringExtra(keyBase + ExtraEventName);
            String description = intent.GetStringExtra(keyBase + ExtraEventDescription);
            DateTime start = DateTime.FromBinary(intent.GetLongExtra(keyBase + ExtraEventStartTime, -1));
            DateTime end = DateTime.FromBinary(intent.GetLongExtra(keyBase + ExtraEventEndTime, -1));

            // Read the event type fields
            String typeName = intent.GetStringExtra(keyBase + ExtraEventTypeName);
            int typePriority = intent.GetIntExtra(keyBase + ExtraEventTypePriority, -1);
            bool typeEditable = intent.GetBooleanExtra(keyBase + ExtraEventTypeEditable, true);
            String typeColor = intent.GetStringExtra(keyBase + ExtraEventTypeColor);
            EventType type = new EventType(typeName, typePriority, typeEditable, typeColor);

            // Return a new Event
            return new Event(name, description, type, start, end);
        }

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