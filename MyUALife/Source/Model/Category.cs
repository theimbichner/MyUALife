using System.Collections.Generic;

public class Category
{
    public static EventType classTime   = new EventType("ClassTime", 0, false, "#F44336");
    public static EventType appointment = new EventType("Appointment", 1, false, "#FF9800");
    public static EventType homework    = new EventType("Homework", 2, true, "#8BC34A");
    public static EventType studyTime   = new EventType("StudyTime", 3, true, "#2196F3");
    public static EventType recreation  = new EventType("Recreation", 4, true, "#673AB7");
    public static EventType freeTime    = new EventType("Free Time", 5, true, "#E91E63");

    public List<EventType> creatableTypes = new List<EventType>(new EventType[] { classTime,
        studyTime, homework, appointment, recreation });
}