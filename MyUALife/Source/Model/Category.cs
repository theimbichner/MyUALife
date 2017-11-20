using System;
using System.Collections.Generic;

namespace MyUALife
{
    public class Category
    {
        public static EventType classTime = new EventType("Class Time", 0, false, "#F44336");
        public static EventType appointment = new EventType("Appointment", 1, false, "#FF9800");
        public static EventType homework = new EventType("Homework", 2, true, "#8BC34A");
        public static EventType studyTime = new EventType("Study Time", 3, true, "#2196F3");
        public static EventType recreation = new EventType("Recreation", 4, true, "#673AB7");
        public static EventType freeTime = new EventType("Free Time", 5, true, "#3F51B5");
        public static EventType other = new EventType("Other", 4, true, "#999999");

        public static List<EventType> creatableTypes = new List<EventType>(new EventType[] { recreation, classTime,
        studyTime, homework, appointment, other });

        public static EventType GetTypeByName(String name)
        {
            foreach (EventType t in creatableTypes)
            {
                if (t.Name == name)
                {
                    return t;
                }
            }
            return freeTime;
        }
    }
}