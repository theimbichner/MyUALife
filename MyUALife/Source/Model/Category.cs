using System;
using System.Collections.Generic;

public class Category
{
    public static EventType classTime = new EventType("ClassTime", false);
    public static EventType studyTime = new EventType("StudyTime", true);
    public static EventType homework = new EventType("Homework", true);
    public static EventType appointment = new EventType("Appointment", false);
    public static EventType recreation = new EventType("Recreation", true);

    List<EventType> theTypes = new List<EventType>(new EventType[] { classTime,
        studyTime, homework, appointment, recreation });
}