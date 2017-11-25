using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using MyUALife;

namespace MyUALifeTests
{
    [TestClass()]
    public class CalendarEventTests
    {
        private static readonly DateTime now = DateTime.Today.AddHours(12);

        private static readonly Event event1 = new Event("Event1", "desc1", Category.Homework,    now.AddMinutes(10), now.AddMinutes(15));
        private static readonly Event event2 = new Event("Event2", "desc2", Category.StudyTime,   now.AddMinutes(20), now.AddMinutes(25));
        private static readonly Event event3 = new Event("Event3", "desc3", Category.ClassTime,   now.AddMinutes(30), now.AddMinutes(35));
        private static readonly Event event4 = new Event("Event4", "desc4", Category.Appointment, now.AddMinutes(40), now.AddMinutes(45));
        private static readonly Event event5 = new Event("Event5", "desc5", Category.Recreation,  now.AddMinutes(50), now.AddMinutes(55));
        private static readonly Event event6 = new Event("Event6", "desc6", Category.Homework,    now.AddMinutes(60), now.AddMinutes(65));
        private static readonly Event event7 = new Event("Event7", "desc7", Category.Other,       now.AddDays(1),     now.AddDays(2));

        private static readonly List<Event> events = new List<Event>
        {
            event1,
            event2,
            event3,
            event4,
            event5,
            event6,
            event7
        };

        [TestMethod()]
        public void TestCalendarGetEventSingle()
        {
            Calendar calendar = new Calendar(CalendarEventTests.events, null);

            var events = calendar.GetEventsInRange(now.AddMinutes(70), now.AddMinutes(75));
            Assert.AreEqual(0, events.Count);

            events = calendar.GetEventsInRange(now.AddMinutes(36), now.AddMinutes(38));
            Assert.AreEqual(0, events.Count);

            events = calendar.GetEventsInRange(now.AddMinutes(0), now.AddMinutes(11));
            Assert.IsTrue(events.Contains(event1));
            Assert.AreEqual(1, events.Count);

            events = calendar.GetEventsInRange(now.AddMinutes(16), now.AddMinutes(21));
            Assert.IsTrue(events.Contains(event2));
            Assert.AreEqual(1, events.Count);

            events = calendar.GetEventsInRange(now.AddMinutes(30), now.AddMinutes(35));
            Assert.IsTrue(events.Contains(event3));
            Assert.AreEqual(1, events.Count);

            events = calendar.GetEventsInRange(now.AddMinutes(39), now.AddMinutes(46));
            Assert.IsTrue(events.Contains(event4));
            Assert.AreEqual(1, events.Count);

            events = calendar.GetEventsInRange(now.AddMinutes(51), now.AddMinutes(54));
            Assert.IsTrue(events.Contains(event5));
            Assert.AreEqual(1, events.Count);

            events = calendar.GetEventsInRange(now.AddMinutes(63), now.AddMinutes(78));
            Assert.IsTrue(events.Contains(event6));
            Assert.AreEqual(1, events.Count);
        }

        [TestMethod()]
        public void TestCalendarGetEventMultiple()
        {
            Calendar calendar = new Calendar(CalendarEventTests.events, null);

            var events = calendar.GetEventsInRange(now.AddMinutes(5), now.AddMinutes(20));
            Assert.IsTrue(events.Contains(event1));
            Assert.IsTrue(events.Contains(event2));
            Assert.AreEqual(2, events.Count);

            events = calendar.GetEventsInRange(now.AddMinutes(35), now.AddMinutes(54));
            Assert.IsTrue(events.Contains(event3));
            Assert.IsTrue(events.Contains(event4));
            Assert.IsTrue(events.Contains(event5));
            Assert.AreEqual(3, events.Count);

            events = calendar.GetEventsInRange(now.AddMinutes(25), now.AddMinutes(40));
            Assert.IsTrue(events.Contains(event2));
            Assert.IsTrue(events.Contains(event3));
            Assert.IsTrue(events.Contains(event4));
            Assert.AreEqual(3, events.Count);

            events = calendar.GetEventsInRange(now.AddMinutes(5), now.AddMinutes(70));
            Assert.IsTrue(events.Contains(event1));
            Assert.IsTrue(events.Contains(event2));
            Assert.IsTrue(events.Contains(event3));
            Assert.IsTrue(events.Contains(event4));
            Assert.IsTrue(events.Contains(event5));
            Assert.IsTrue(events.Contains(event6));
            Assert.AreEqual(6, events.Count);
        }

        [TestMethod()]
        public void TestCalendarGetEventDate()
        {
            Calendar calendar = new Calendar(CalendarEventTests.events, null);

            var events = calendar.GetEventsOnDate(now);
            Assert.IsTrue(events.Contains(event1));
            Assert.IsTrue(events.Contains(event2));
            Assert.IsTrue(events.Contains(event3));
            Assert.IsTrue(events.Contains(event4));
            Assert.IsTrue(events.Contains(event5));
            Assert.IsTrue(events.Contains(event6));
            Assert.AreEqual(6, events.Count);
        }

        [TestMethod()]
        public void TestCalendarGetDeadline()
        {
            Deadline deadline1 = new Deadline("name", "desc", now.AddMinutes(10));
            Deadline deadline2 = new Deadline("name", "desc", now.AddMinutes(20));
            Deadline deadline3 = new Deadline("name", "desc", now.AddMinutes(30));
            Calendar calendar = new Calendar();
            calendar.AddDeadline(deadline1);
            calendar.AddDeadline(deadline2);
            calendar.AddDeadline(deadline3);

            var deadlines = calendar.GetDeadlinesAfterTime(now.AddMinutes(15));
            Assert.AreEqual(2, deadlines.Count);
            Assert.IsTrue(deadlines.Contains(deadline2));
            Assert.IsTrue(deadlines.Contains(deadline3));
        }
    }
}