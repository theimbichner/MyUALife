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

    public String Description
    {
        get;
        set;
    }

    public EventType Type
    {
        get;
        set;
    }

    public Event(String name, String desc, EventType type, DateTime start, DateTime end)
    {
        Name = name;
        Description = desc;
        Type = type;
        StartTime = start;
        EndTime = end;
    }
}