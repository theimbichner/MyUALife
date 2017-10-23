using System;

public struct EventType
{
    String name;
    bool editable;
    int priority;

    public EventType(String str, int priority, bool ed)
    {
        name = str;
        this.priority = priority;
        editable = ed;
    }
}