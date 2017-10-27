using System;

public struct EventType
{
    public readonly String name;
    public readonly bool editable;
    public readonly int priority;
    public readonly String colorString;

    public EventType(String str, int priority, bool ed, String color)
    {
        name = str;
        this.priority = priority;
        editable = ed;
        colorString = color;
    }
}