using System;
using System.Collections.Generic;

namespace MyUALife
{
    public static class Category
    {
        public static EventType ClassTime   = new EventType("Class Time",  0, false, "#F44336");
        public static EventType Appointment = new EventType("Appointment", 1, false, "#FF9800");
        public static EventType Homework    = new EventType("Homework",    2, true,  "#8BC34A");
        public static EventType StudyTime   = new EventType("Study Time",  3, true,  "#2196F3");
        public static EventType Recreation  = new EventType("Recreation",  4, true,  "#673AB7");
        public static EventType Other       = new EventType("Other",       4, true,  "#9e9e9e");
        public static EventType FreeTime    = new EventType("Free Time",   5, true,  "#3F51B5");

        public readonly static List<EventType> CreatableTypes = new List<EventType>
        {
            Recreation,
            ClassTime,
            StudyTime,
            Homework,
            Appointment,
            Other
        };

        public static EventType GetTypeByName(String name)
        {
            foreach (EventType t in CreatableTypes)
            {
                if (t.Name == name)
                {
                    return t;
                }
            }
            return FreeTime;
        }
    }
}