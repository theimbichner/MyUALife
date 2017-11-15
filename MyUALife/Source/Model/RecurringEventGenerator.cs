﻿using System;
using System.Collections.Generic;

namespace MyUALife
{
    [Serializable()]
    public class RecurringEventGenerator
    {
        // Events starting before or on this time have already been generated.
        private DateTime updatedThrough;

        // The prefab for all generated Events.
        private readonly Event baseEvent;

        // The spacing between Events.
        private readonly TimeSpan spacing;

        /*
         * Constructs a generator for events that resemble baseEvent and who
         * recur with the given spacing. The baseEvent will not be generated by
         * any call to GenerateEvents().
         */
        public RecurringEventGenerator(Event baseEvent, TimeSpan spacing)
        {
            this.baseEvent = baseEvent;
            this.spacing = spacing;

            updatedThrough = baseEvent.StartTime;
        }

        /*
         * Constructor for serialization.
         */
        private RecurringEventGenerator() { }

        /*
         * Create Events that start on or before the specified time and that
         * have not been created before.
         */
        public List<Event> GenerateEvents(DateTime time)
        {
            // We've already generated the Events, so return nothing.
            if (time <= updatedThrough)
            {
                return new List<Event>();
            }
            
            DateTime startTime = baseEvent.StartTime;
            int shifts = 0;

            // Get the first event that has not yet been generated.
            while (startTime <= updatedThrough)
            {
                startTime += spacing;
                shifts++;
            }

            TimeSpan initShift = TimeSpan.FromTicks(spacing.Ticks * shifts);
            Event calendarEvent = baseEvent.Shift(initShift);
            List<Event> generatedEvents = new List<Event>();

            // Add all the necessary new Events
            while (calendarEvent.StartTime <= time)
            {
                generatedEvents.Add(calendarEvent);
                calendarEvent = calendarEvent.Shift(spacing);
            }
            updatedThrough = time;
            return generatedEvents;
        }
    }
}