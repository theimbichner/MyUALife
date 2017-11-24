using Android.Content;
using System;

namespace MyUALife
{
    [Serializable()]
    public class Deadline : IComparable<Deadline>
    {
        // Key fragments for specifying individual fields of Deadlines
        private const String ExtraDeadlineName = ".DeadlineName";
        private const String ExtraDeadlineDescription = ".DeadlineDescription";
        private const String ExtraDeadlineTime = ".DeadlineTime";

        // Key fragments for specifying fields of the Deadline type
        private const String ExtraDeadlineTypeName = ".DeadlineType.Name";
        private const String ExtraDeadlineTypePriority = ".DeadlineType.Priority";
        private const String ExtraDeadlineTypeEditable = ".DeadlineType.Editable";
        private const String ExtraDeadlineTypeColor = ".DeadlineType.Color";

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
            this(name, desc, time, Category.Other) { }

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
         * Writes the given Deadline to the intent using the supplied key.
         */
        public static void WriteDeadline(Intent intent, String keyBase, Deadline calendarDeadline)
        {
            // Write the Deadline fields
            intent.PutExtra(keyBase + ExtraDeadlineName, calendarDeadline.Name);
            intent.PutExtra(keyBase + ExtraDeadlineDescription, calendarDeadline.Description);
            intent.PutExtra(keyBase + ExtraDeadlineTime, calendarDeadline.Time.ToBinary());

            // Write the Deadline type fields
            intent.PutExtra(keyBase + ExtraDeadlineTypeName, calendarDeadline.Type.Name);
            intent.PutExtra(keyBase + ExtraDeadlineTypePriority, calendarDeadline.Type.Priority);
            intent.PutExtra(keyBase + ExtraDeadlineTypeEditable, calendarDeadline.Type.IsEditable);
            intent.PutExtra(keyBase + ExtraDeadlineTypeColor, calendarDeadline.Type.ColorString);
        }

        /*
         * Reads the Deadline associated with the given key from the intent.
         */
        public static Deadline ReadDeadline(Intent intent, String keyBase)
        {
            // Return null in the case that there is no Deadline
            if (!intent.HasExtra(keyBase + ExtraDeadlineName))
            {
                return null;
            }

            // Read the Deadline fields
            String name = intent.GetStringExtra(keyBase + ExtraDeadlineName);
            String description = intent.GetStringExtra(keyBase + ExtraDeadlineDescription);
            DateTime time = DateTime.FromBinary(intent.GetLongExtra(keyBase + ExtraDeadlineTime, -1));

            // Read the Deadline type fields
            String typeName = intent.GetStringExtra(keyBase + ExtraDeadlineTypeName);
            int typePriority = intent.GetIntExtra(keyBase + ExtraDeadlineTypePriority, -1);
            bool typeEditable = intent.GetBooleanExtra(keyBase + ExtraDeadlineTypeEditable, true);
            String typeColor = intent.GetStringExtra(keyBase + ExtraDeadlineTypeColor);
            EventType type = new EventType(typeName, typePriority, typeEditable, typeColor);

            // Return a new Deadline
            return new Deadline(name, description, time, type);
        }

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