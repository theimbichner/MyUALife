using System;
using System.Collections.Generic;

namespace MyUALife
{
    class FilterSet
    {
        // The available FilterSets
        private readonly static FilterSet allTypes   = new FilterSet("All types", Category.creatableTypes);
        private readonly static FilterSet recreation = new FilterSet("Recreation", Category.recreation);
        private readonly static FilterSet academics  = new FilterSet("Academics", Category.classTime, Category.homework);

        // List containing all FilterSets
        public static List<FilterSet> FilterSets
        {
            get
            {
                return new List<FilterSet> { allTypes, recreation, academics };
            }
        }

        // A name for this FilterSet
        public String Name { get; set; }

        // The list of types in this FilterSet
        private List<EventType> allowedTypes;
        public List<EventType> AllowedTypes
        {
            get
            {
                var list = new List<EventType>();
                list.AddRange(allowedTypes);
                return list;
            }
        }

        /*
         * Creates a FilterSet from an array of EventTypes.
         */
        private FilterSet(String name, params EventType[] types)
        {
            allowedTypes = new List<EventType>();
            foreach (EventType t in types)
            {
                allowedTypes.Add(t);
            }
            Name = name;
        }

        /*
         * Creates a FilterSet from a list of EventTypes.
         */
        private FilterSet(String name, List<EventType> types)
        {
            allowedTypes = new List<EventType>();
            allowedTypes.AddRange(types);
            Name = name;
        }
    }
}