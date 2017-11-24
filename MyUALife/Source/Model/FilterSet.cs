using System;
using System.Collections.Generic;

namespace MyUALife
{
    public class FilterSet
    {
        // The available FilterSets
        private readonly static FilterSet allTypes = new FilterSet("All types", Category.CreatableTypes);
        private readonly static FilterSet recreation = new FilterSet("Recreation", Category.Recreation);
        private readonly static FilterSet academics = new FilterSet("Academics", Category.ClassTime, Category.Homework, Category.StudyTime);
        private readonly static FilterSet appointments = new FilterSet("Appointments", Category.Appointment);

        // List containing all FilterSets
        public readonly static List<FilterSet> FilterSets = new List<FilterSet> { allTypes, recreation, academics, appointments };

        /*
         * Creates a FilterSet from an array of EventTypes.
         */
        private FilterSet(String name, params EventType[] types)
        {
            AllowedTypes = new List<EventType>();
            foreach (EventType t in types)
            {
                AllowedTypes.Add(t);
            }
            Name = name;
        }

        /*
         * Creates a FilterSet from a list of EventTypes.
         */
        private FilterSet(String name, List<EventType> types)
        {
            AllowedTypes = new List<EventType>();
            foreach (EventType t in types)
            {
                AllowedTypes.Add(t);
            }
            Name = name;
        }

        // A name for this FilterSet
        public String Name { get; set; }

        // The types allowed by this filter set
        public List<EventType> AllowedTypes { get; private set; }
    }
}