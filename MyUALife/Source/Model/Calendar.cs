using System;
using System.Collections.Generic;

namespace MyUALife
{
    [Serializable()]
    public class Calendar
    {
        // The Events stored in the Calendar
        private readonly List<Event> events = new List<Event>();

        // The Deadlines stored in the Calendar
        private readonly List<Deadline> deadlines = new List<Deadline>();

        // Generators for all the recurring Events
        private readonly List<RecurringEventGenerator> recurringEvents = new List<RecurringEventGenerator>();

        /*
         * Default constructor. Creates a calendar with no events or deadlines.
         */
        public Calendar() { }

        /*
         * Constructs a Calendar initialized to contain the given events and deadlines.
         */
        public Calendar(List<Event> events, List<Deadline> deadlines)
        {
            this.events.AddRange(events ?? new List<Event>());
            this.deadlines.AddRange(deadlines ?? new List<Deadline>());
        }

        /*
         * Creates a Calendar with some sample events and deadlines, for ease
         * of testing.
         */
        public static Calendar CreateDefaultCalendar()
        {
            String classDesc       = "Software Engineering";
            String appointmentDesc = "This is an appointment with an advisor.";
            String hwDesc          = "This is homework for CS436";
            String studyDesc       = "This is study time.";
            String recDesc         = "This is a recreational event.";

            Calendar ret = new Calendar();

            ret.AddEvent(new Event("CS436 Class",          classDesc,       Category.ClassTime,   Time(12, 30), Time(13, 45)));
            ret.AddEvent(new Event("Advising Appointment", appointmentDesc, Category.Appointment, Time(14, 30), Time(15, 0)));
            ret.AddEvent(new Event("Build Android App",    hwDesc,          Category.Homework,    Time(15, 15), Time(17, 0)));
            ret.AddEvent(new Event("Study",                studyDesc,       Category.StudyTime,   Time(10, 0),  Time(12, 15)));
            ret.AddEvent(new Event("Play Videogames",      recDesc,         Category.Recreation,  Time(18, 25), Time(23, 59)));

            ret.AddDeadline(new Deadline("Test Deadline", "An example of a deadline.", Time(24, 0)));

            return ret;
        }

        /*
         * Returns a DateTime representing today at the given time of day.
         */
        private static DateTime Time(int hours, int mins)
        {
            return DateTime.Today.AddHours(hours).AddMinutes(mins);
        }

        /*
         * Given a List of Events, returns a new List containing only the
         * Events of EventType type.
         */
        public static List<Event> FilterEventsByType(List<Event> events, EventType type)
        {
            return events.FindAll(e => e.Type.Equals(type));
        }

        /*
         * Given a List of Events, returns a new List containing only the
         * Events whose EventType is one of the elements of types.
         */
        public static List<Event> FilterEventsByTypes(List<Event> events, List<EventType> types)
        {
            return events.FindAll(e => types.Contains(e.Type));
        }

        /*
         * Adds the specified Event to the Calendar.
         */
        public void AddEvent(Event e)
        {
            events.Add(e);
        }

        /*
         * Removes the specified Event from the Calendar. If the Event did not
         * previously exist in the Calendar, returns false. Otherwise, returns
         * true.
         */
        public bool RemoveEvent(Event e)
        {
            return events.Remove(e);
        }

        /*
         * Adds a recurring Event to the Calendar.
         */
        public void AddRecurringEvent(RecurringEventGenerator er)
        {
            recurringEvents.Add(er);
        }

        /*
         * Adds the specified Deadline to the Calendar.
         */
        public void AddDeadline(Deadline d)
        {
            deadlines.Add(d);
        }

        /*
         * Removes the specified Deadline from the Calendar. Returns false if
         * the Deadline did not previously exist in the Calendar, and true
         * otherwise.
         */
        public bool RemoveDeadline(Deadline d)
        {
            return deadlines.Remove(d);
        }

        /*
         * Returns a List containing all the Events in the calendar who
         * intersect the day on which the specified DateTime falls.
         */
        public List<Event> GetEventsOnDate(DateTime time)
        {
            DateTime date = time.Date;

            DateTime start = date.AddMilliseconds(1);
            DateTime end = date.AddDays(1);

            return GetEventsInRange(start, end);
        }

        /*
         * Returns a List containing all the Events in the calendar who
         * intersect the range of time starting at start and ending at end.
         */
        public List<Event> GetEventsInRange(DateTime start, DateTime end)
        {
            foreach (RecurringEventGenerator re in recurringEvents)
            {
                var events = re.GenerateEvents(end);
                foreach (Event e in events)
                {
                    AddEvent(e);
                }
            }

            List<Event> output = new List<Event>();
            foreach (Event e in events)
            {
                if (e.StartTime <= end && e.EndTime >= start)
                {
                    output.Add(e);
                }
            }
            return output;
        }

        /*
         * Returns a List containing all the Deadlines in the Calendar whose
         * end times come after the specified DateTime.
         */
        public List<Deadline> GetDeadlinesAfterTime(DateTime time)
        {
            return deadlines.FindAll(d => d.Time > time);
        }

        /*
         * Returns a List containing Events representing the empty space on the
         * Calendar on the day on which the specified DateTime falls.
         */
        public List<Event> GetFreeTimeOnDate(DateTime time)
        {
            DateTime date = time.Date;

            DateTime start = date.AddMilliseconds(1);
            DateTime end = date.AddDays(1);

            return GetFreeTimeBlocksInRange(start, end);
        }

        /*
         * Returns a List containing Events representing the empty space on the
         * Calendar between start and end.
         */
        public List<Event> GetFreeTimeBlocksInRange(DateTime start, DateTime end)
        {
            List<Event> freeBlocks = new List<Event>();
            List<Event> eventsInRange = GetEventsInRange(start, end);
            DateTime currentTime = start;

            while (currentTime < end)
            {
                bool freeTimeAvailableHere = true;

                // search for any events occupying current time
                foreach (Event e in eventsInRange)
                {
                    if (e.StartTime <= currentTime && e.EndTime > currentTime)
                    {
                        // if found, skip to the end of the event and check again
                        currentTime = e.EndTime;
                        freeTimeAvailableHere = false;
                    }
                }

                if (freeTimeAvailableHere)
                {
                    // no events here, so:
                    // find next event (earliest starting after currentTime)
                    Event nextEvent = null;
                    foreach (Event e in eventsInRange)
                    {
                        if (e.StartTime > currentTime)
                        {
                            if (nextEvent == null || e.StartTime < nextEvent.StartTime)
                            {
                                nextEvent = e;
                            }
                        }
                    }

                    // create the free time block and add it to freeBlocks:
                    // if no events start after currentTime, end time = end
                    if (nextEvent == null)
                    {
                        if ((end - currentTime).Duration().TotalMinutes > 0)
                        {
                            freeBlocks.Add(new Event("Free", "", Category.FreeTime, currentTime, end));
                        }
                        currentTime = end;
                    }
                    // otherwise, end time = nextEvent.StartTime
                    else
                    {
                        if ((nextEvent.StartTime - currentTime).Duration().TotalMinutes > 0)
                        {
                            freeBlocks.Add(new Event("Free", "", Category.FreeTime, currentTime, nextEvent.StartTime));
                        }
                        currentTime = nextEvent.EndTime;
                    }

                }

            }

            return freeBlocks;
        }
    }
}