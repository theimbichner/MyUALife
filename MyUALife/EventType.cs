using System;

public struct EventType
{
    String name;
    bool editable;

    public EventType(String str, bool ed)
    {
        name = str;
        editable = ed;
    }
}