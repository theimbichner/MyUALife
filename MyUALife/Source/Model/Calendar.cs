using System;
using System.Collections.Generic;

namespace MyUALife
{
    public class Calendar
    {
        private readonly List<Event> events = new List<Event>();
        private readonly List<Deadline> deadlines = new List<Deadline>();
        private readonly List<Event> happenings = new List<Event>();
        private readonly List<RecurringEventGenerator> recurringEvents = new List<RecurringEventGenerator>();

        public Calendar() { }

        public Calendar(List<Event> events, List<Deadline> deadlines)
        {
            this.events = events;
            this.deadlines = deadlines;
        }

        private static void Init()
        {
            Calendar Calendar = new Calendar();

            DateTime time = DateTime.Now;
            DateTime midnightMorning = DateTime.Today;
            DateTime midnightNight = DateTime.Today.AddDays(1);

            String classDesc = "Software Engineering";
            String appointmentDesc = "This is an appointment with a professor.";
            String hwDesc = "This is homework for CS436";
            String studyDesc = "This is study time.";
            String recDesc = "This is a recreational event.";
            // String freeDesc        = "This is free time. This category should not appear in user added events.";
            // String lastNightDesc   = "This was midnight last night. This should appear as part of yesterday.";
            // String tonightDesc     = "This is midnight tonight. This should appear as part of today.";

            Calendar.AddEvent(new Event("CS436 Class", classDesc, Category.classTime, Time(12, 30), Time(13, 45)));
            Calendar.AddEvent(new Event("Office Hours", appointmentDesc, Category.appointment, Time(14, 30), Time(15, 0)));
            Calendar.AddEvent(new Event("Build Android App", hwDesc, Category.homework, Time(15, 15), Time(17, 0)));
            Calendar.AddEvent(new Event("Study", studyDesc, Category.studyTime, Time(10, 0), Time(12, 15)));
            Calendar.AddEvent(new Event("Play Videogames", recDesc, Category.recreation, Time(18, 25), Time(23, 59)));
            // Calendar.AddEvent(new Event("???",               freeDesc,        Category.freeTime,    time, time));
            // Calendar.AddEvent(new Event("Last Midnight",     lastNightDesc,   Category.freeTime,    midnightMorning, midnightMorning));
            // Calendar.AddEvent(new Event("Midnight Tonight",  tonightDesc,     Category.freeTime,    midnightNight, midnightNight));

            Calendar.AddDeadline(new Deadline("Deadline", "It's a deadline!!!!!!!!!!", midnightNight));
        }

        static DateTime Time(int hours, int mins)
        {
            return DateTime.Today.AddHours(hours).AddMinutes(mins);
        }

        public static List<Event> FilterEventsByType(List<Event> events, EventType type)
        {
            List<Event> output = new List<Event>();
            foreach (Event e in events)
            {
                if (e.Type.Equals(type))
                {
                    output.Add(e);
                }
            }
            return output;
        }

        public static List<Event> FilterEventsByTypes(List<Event> events, List<EventType> types)
        {
            List<Event> output = new List<Event>();
            foreach (Event e in events)
            {
                if (types.Contains(e.Type))
                {
                    output.Add(e);
                }
            }
            return output;
        }

        public void AddEvent(Event e)
        {
            events.Add(e);
        }

        public bool RemoveEvent(Event e)
        {
            return events.Remove(e);
        }

        public void AddRecurringEvent(RecurringEventGenerator er)
        {
            recurringEvents.Add(er);
        }

        public bool CancelRecurringEvent(RecurringEventGenerator er)
        {
            return recurringEvents.Remove(er);
        }

        public void AddDeadline(Deadline d)
        {
            deadlines.Add(d);
        }

        public bool RemoveDeadline(Deadline d)
        {
            return deadlines.Remove(d);
        }

        public void AddHappening(Event e)
        {
            happenings.Add(e);
        }

        public bool RemoveHappening(Event e)
        {
            return happenings.Remove(e);
        }

        public List<Event> GetEventsOnDate(DateTime date)
        {
            // Create a range of DateTimes
            // We want to count midnight as belonging to the previous day.
            DateTime start = date.AddMilliseconds(1);
            DateTime end = date.AddDays(1);

            return GetEventsInRange(start, end);
        }

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

        public List<Deadline> GetDeadlinesAfterTime(DateTime time)
        {
            List<Deadline> output = new List<Deadline>();
            foreach (Deadline d in deadlines)
            {
                if (d.Time > time)
                {
                    output.Add(d);
                }
            }
            return output;
        }

        public List<Event> GetFreeTimeOnDate(DateTime date)
        {
            // Create a range of DateTimes
            // We want to count midnight as belonging to the previous day.
            DateTime start = date.AddMilliseconds(1);
            DateTime end = date.AddDays(1);

            return GetFreeTimeBlocksInRange(start, end);
        }

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
                            freeBlocks.Add(new Event("Free", "", Category.freeTime, currentTime, end));
                        }
                        currentTime = end;
                    }
                    // otherwise, end time = nextEvent.StartTime
                    else
                    {
                        if ((nextEvent.StartTime - currentTime).Duration().TotalMinutes > 0)
                        {
                            freeBlocks.Add(new Event("Free", "", Category.freeTime, currentTime, nextEvent.StartTime));
                        }
                        currentTime = nextEvent.EndTime;
                    }

                }

            }

            return freeBlocks;
        }

    }
}