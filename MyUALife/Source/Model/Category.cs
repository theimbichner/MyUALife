using System.Collections.Generic;

public class Category
{
    public static EventType classTime   = new EventType("ClassTime", 0, false);
    public static EventType appointment = new EventType("Appointment", 1, false);
    public static EventType homework    = new EventType("Homework", 2, true);
    public static EventType studyTime   = new EventType("StudyTime", 3, true);
    public static EventType recreation  = new EventType("Recreation", 4, true);
    public static EventType freeTime    = new EventType("Free Time", 5, true);

    public List<EventType> creatableTypes = new List<EventType>(new EventType[] { classTime,
        studyTime, homework, appointment, recreation });
}