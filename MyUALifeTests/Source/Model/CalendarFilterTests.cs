using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using MyUALife;

namespace MyUALifeTests
{
    [TestClass()]
    public class CalendarFilterTests
    {
        private static readonly DateTime now = DateTime.Now;

        private static readonly Event event1 = new Event("Event1", "desc1", Category.Homework,    now.AddMinutes(10), now.AddMinutes(15));
        private static readonly Event event2 = new Event("Event2", "desc2", Category.StudyTime,   now.AddMinutes(20), now.AddMinutes(25));
        private static readonly Event event3 = new Event("Event3", "desc3", Category.ClassTime,   now.AddMinutes(30), now.AddMinutes(35));
        private static readonly Event event4 = new Event("Event4", "desc4", Category.Appointment, now.AddMinutes(40), now.AddMinutes(45));
        private static readonly Event event5 = new Event("Event5", "desc5", Category.Recreation,  now.AddMinutes(50), now.AddMinutes(55));
        private static readonly Event event6 = new Event("Event6", "desc6", Category.Homework,    now.AddMinutes(60), now.AddMinutes(65));

        private static readonly List<Event> events = new List<Event>
        {
            event1,
            event2,
            event3,
            event4,
            event5,
            event6
        };

        [TestMethod()]
        public void TestCalendarFilterSingle()
        {
            var filtered = Calendar.FilterEventsByType(events, Category.Homework);
            Assert.IsTrue(filtered.Contains(event1));
            Assert.IsTrue(filtered.Contains(event6));
            Assert.AreEqual(2, filtered.Count);

            filtered = Calendar.FilterEventsByType(events, Category.StudyTime);
            Assert.IsTrue(filtered.Contains(event2));
            Assert.AreEqual(1, filtered.Count);

            filtered = Calendar.FilterEventsByType(events, Category.ClassTime);
            Assert.IsTrue(filtered.Contains(event3));
            Assert.AreEqual(1, filtered.Count);

            filtered = Calendar.FilterEventsByType(events, Category.Appointment);
            Assert.IsTrue(filtered.Contains(event4));
            Assert.AreEqual(1, filtered.Count);

            filtered = Calendar.FilterEventsByType(events, Category.Recreation);
            Assert.IsTrue(filtered.Contains(event5));
            Assert.AreEqual(1, filtered.Count);

            filtered = Calendar.FilterEventsByType(events, Category.FreeTime);
            Assert.AreEqual(0, filtered.Count);
        }

        [TestMethod()]
        public void TestCalendarFilterList()
        {
            List<EventType> filters = new List<EventType> { Category.Homework, Category.ClassTime };
            var filtered = Calendar.FilterEventsByTypes(events, filters);
            Assert.IsTrue(filtered.Contains(event1));
            Assert.IsTrue(filtered.Contains(event3));
            Assert.IsTrue(filtered.Contains(event6));
            Assert.AreEqual(3, filtered.Count);

            filtered = Calendar.FilterEventsByTypes(events, Category.CreatableTypes);
            Assert.AreEqual(events.Count, filtered.Count);
            foreach (Event e in events)
            {
                Assert.IsTrue(filtered.Contains(e));
            }

            filtered = Calendar.FilterEventsByTypes(events, new List<EventType>());
            Assert.AreEqual(0, filtered.Count);
        }
    }
}