using System;

public class Event : IComparable<Event>
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

    public override String ToString()
    {
        string format = "Name: {0}\nDescription: {1}\nFrom: {2}\nTo: {3}\nType: {4}";
        object[] args = {Name, Description, StartTime, EndTime, Type.name};
        return String.Format(format, args);
    }

    public int CompareTo(Event e)
    {
        return StartTime.CompareTo(e.StartTime);
    }
}