using Android.Content;
using System;

namespace MyUALife
{
    public class DeadlineSerializer
    {
        // Key fragments for distinguishing between input and output Deadlines
        public const String InputDeadline = "MyUALife.InputDeadline";
        public const String ResultDeadline = "MyUALife.ResultDeadline";

        // Key fragments for specifying individual fields of Deadlines
        private const String ExtraDeadlineName = ".DeadlineName";
        private const String ExtraDeadlineDescription = ".DeadlineDescription";
        private const String ExtraDeadlineTime = ".DeadlineTime";

        // Key fragments for specifying fields of the Deadline type
        private const String ExtraDeadlineTypeName = ".DeadlineType.Name";
        private const String ExtraDeadlineTypePriority = ".DeadlineType.Priority";
        private const String ExtraDeadlineTypeEditable = ".DeadlineType.Editable";
        private const String ExtraDeadlineTypeColor = ".DeadlineType.Color";

        private readonly Intent intent;

        /*
         * Creates an DeadlineSerializer for reading and writing Deadlines to and
         * from the given Intent.
         */
        public DeadlineSerializer(Intent intent)
        {
            this.intent = intent;
        }

        /*
         * Writes the given Deadline to the intent using the supplied key.
         */
        public void WriteDeadline(String keyBase, Deadline calendarDeadline)
        {
            // Write the Deadline fields
            intent.PutExtra(keyBase + ExtraDeadlineName, calendarDeadline.Name);
            intent.PutExtra(keyBase + ExtraDeadlineDescription, calendarDeadline.Description);
            intent.PutExtra(keyBase + ExtraDeadlineTime, calendarDeadline.Time.ToBinary());

            // Write the Deadline type fields
            intent.PutExtra(keyBase + ExtraDeadlineTypeName, calendarDeadline.associatedEventType.name);
            intent.PutExtra(keyBase + ExtraDeadlineTypePriority, calendarDeadline.associatedEventType.priority);
            intent.PutExtra(keyBase + ExtraDeadlineTypeEditable, calendarDeadline.associatedEventType.editable);
            intent.PutExtra(keyBase + ExtraDeadlineTypeColor, calendarDeadline.associatedEventType.colorString);
        }

        /*
         * Reads the Deadline associated with the given key from the intent.
         */
        public Deadline ReadDeadline(String keyBase)
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
    }
}