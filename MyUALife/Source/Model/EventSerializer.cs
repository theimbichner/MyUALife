using Android.Content;
using System;

namespace MyUALife
{
    public class EventSerializer
    {
        // Key fragments for distinguishing between input and output events
        public const String InputEvent = "MyUALife.InputEvent";
        public const String ResultEvent = "MyUALife.ResultEvent";

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

        private readonly Intent intent;

        /*
         * Creates an EventSerializer for reading and writing Events to and
         * from the given Intent.
         */
        public EventSerializer(Intent intent)
        {
            this.intent = intent;
        }

        /*
         * Writes the given event to the intent using the supplied key.
         */
        public void WriteEvent(String keyBase, Event calendarEvent)
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
        public Event ReadEvent(String keyBase)
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
    }
}