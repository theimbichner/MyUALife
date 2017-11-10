﻿using System;
using System.Collections.Generic;

[Serializable()]
public class Calendar
{
    List<Event> events = new List<Event>();
    List<Deadline> deadlines = new List<Deadline>();
    List<Event> happenings = new List<Event>();

    public Calendar()
    {

    }

    public Calendar(List<Event> events, List<Deadline> deadlines)
    {
        this.events = events;
        this.deadlines = deadlines;
    }

    public void AddEvent(Event e)
    {
        events.Add(e);
    }

    public bool RemoveEvent(Event e)
    {
        return events.Remove(e);
    }

	public void AddDeadline(Deadline d) {
		deadlines.Add(d);
	}

	public bool RemoveDeadline(Deadline d) {
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

    public List<Event> FilterEventsByType(List<Event> events, EventType type)
    {
        List<Event> output = new List<Event>();
        foreach (Event e in events)
        {
            if (e.Type.Equals(type)) output.Add(e);
        }
        return output;
    }

    public List<Event> FilterEventsByTypes(List<Event> events, List<EventType> types)
    {
        List<Event> output = new List<Event>();
        foreach (Event e in events)
        {
            if (types.Contains(e.Type)) output.Add(e);
        }
        return output;
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
        List<Event> output = new List<Event>();
        foreach(Event e in events)
        {
            if (e.StartTime <= end && e.EndTime >= start)
            {
                output.Add(e);
            }
        }
        return output;
    }

	public List<Deadline> GetDeadlinesAfterTime(DateTime time) {
		List<Deadline> output = new List<Deadline>();
		foreach (Deadline d in deadlines) {
			if (d.Time > time) output.Add(d);
		}
		return output;
	}

	public List<Event> GetFreeTimeBlocksInRange(DateTime start, DateTime end)
	{
		List<Event> freeBlocks = new List<Event>();
		List<Event> eventsInRange = GetEventsInRange(start, end);
		DateTime currentTime = start;

		while (currentTime < end) {

			bool freeTimeAvailableHere = true;
			
			// search for any events occupying current time
			foreach (Event e in eventsInRange) {
				if (e.StartTime <= currentTime && e.EndTime > currentTime) {
					// if found, skip to the end of the event and check again
					currentTime = e.EndTime;
					freeTimeAvailableHere = false;
				}
			}

			if (freeTimeAvailableHere) {
				// no events here, so:
				// find next event (earliest starting after currentTime)
				Event nextEvent = null;
				foreach (Event e in eventsInRange) {
					if (e.StartTime > currentTime) {
						if (nextEvent == null || e.StartTime < nextEvent.StartTime) {
							nextEvent = e;
						}
					}
				}

				// create the free time block and add it to freeBlocks:
				// if no events start after currentTime, end time = end
				if (nextEvent == null) {
					if ((end-currentTime).Duration().TotalMinutes > 0)
						freeBlocks.Add(new Event("Free", "", Category.freeTime, currentTime, end));
					currentTime = end;
				}
				// otherwise, end time = nextEvent.StartTime
				else {
					if ((nextEvent.StartTime-currentTime).Duration().TotalMinutes > 0)
						freeBlocks.Add(new Event("Free", "", Category.freeTime, currentTime, nextEvent.StartTime));
					currentTime = nextEvent.EndTime;
				}

			}

		}

		return freeBlocks;
	}

}