/* Team 7: MyUALife
 * Daniel Alexander, Ryan Avila, Peter Chipman, Taylor Heimbichner
 * Class: Calendar
 */

using System;
using System.Collections.Generic;

public class Calendar
{
    List<Event> events = new List<Event>();

    public Calendar()
    {

    }

    public Calendar(List<Event> events)
    {
        this.events = events;
    }

    public void AddEvent(Event e)
    {
        events.Add(e);
    }

    public bool RemoveEvent(Event e)
    {
        return events.Remove(e);
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




}