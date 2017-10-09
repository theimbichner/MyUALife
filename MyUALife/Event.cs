/* Team 7: MyUALife
 * Daniel Alexander, Ryan Avila, Peter Chipman, Taylor Heimbichner
 * Class: Event
 */

using System;

public class Event
{
    public DateTime StartTime
    {
        get;
        set;
    }

    public DateTime EndTime
    {
        get;
        set;
    }

    public String Name
    {
        get;
        set;
    }

    public Event(String name, DateTime start, DateTime end)
    {
        Name = name;
        StartTime = start;
        EndTime = end;
    }
}