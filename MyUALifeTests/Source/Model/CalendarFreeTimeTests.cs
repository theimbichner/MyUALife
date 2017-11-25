using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyUALife;
using System;
using System.Collections.Generic;

namespace MyUALifeTests
{
    [TestClass()]
    public class CalendarFreeTimeTests
    {
        private static readonly DateTime now = now = DateTime.Today.AddHours(12);

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
        public void TestFreeTime()
        {
            Calendar calendar = new Calendar(events, null);
            var freeTime = calendar.GetFreeTimeBlocksInRange(now.AddMinutes(7), now.AddMinutes(27));
            freeTime.Sort();
            Assert.AreEqual(3, freeTime.Count);
            foreach (Event e in freeTime)
            {
                Assert.AreEqual(Category.FreeTime, e.Type);
            }

            Assert.AreEqual(now.AddMinutes(7), freeTime[0].StartTime);
            Assert.AreEqual(now.AddMinutes(10), freeTime[0].EndTime);

            Assert.AreEqual(now.AddMinutes(15), freeTime[1].StartTime);
            Assert.AreEqual(now.AddMinutes(20), freeTime[1].EndTime);

            Assert.AreEqual(now.AddMinutes(25), freeTime[2].StartTime);
            Assert.AreEqual(now.AddMinutes(27), freeTime[2].EndTime);
        }

        [TestMethod()]
        public void TestFreeTimeEmpty()
        {
            Calendar calendar = new Calendar(events, null);
            var freeTime = calendar.GetFreeTimeBlocksInRange(now.AddMinutes(10), now.AddMinutes(15));
            Assert.AreEqual(0, freeTime.Count);

            freeTime = calendar.GetFreeTimeBlocksInRange(now.AddMinutes(21), now.AddMinutes(24));
            Assert.AreEqual(0, freeTime.Count);
        }

        [TestMethod()]
        public void TestFreeTimeBounded()
        {
            Calendar calendar = new Calendar(events, null);
            var freeTime = calendar.GetFreeTimeBlocksInRange(now.AddMinutes(12), now.AddMinutes(33));
            freeTime.Sort();
            Assert.AreEqual(2, freeTime.Count);
            foreach (Event e in freeTime)
            {
                Assert.AreEqual(Category.FreeTime, e.Type);
            }

            Assert.AreEqual(now.AddMinutes(15), freeTime[0].StartTime);
            Assert.AreEqual(now.AddMinutes(20), freeTime[0].EndTime);

            Assert.AreEqual(now.AddMinutes(25), freeTime[1].StartTime);
            Assert.AreEqual(now.AddMinutes(30), freeTime[1].EndTime);
        }

        [TestMethod()]
        public void TestFreeTimeDate()
        {
            Calendar calendar = new Calendar(events, null);
            var freeTime = calendar.GetFreeTimeOnDate(now);
            freeTime.Sort();
            Assert.AreEqual(7, freeTime.Count);
            foreach (Event e in freeTime)
            {
                Assert.AreEqual(Category.FreeTime, e.Type);
            }

            Assert.AreEqual(now.Date.AddMilliseconds(1), freeTime[0].StartTime);
            Assert.AreEqual(now.AddMinutes(10), freeTime[0].EndTime);

            Assert.AreEqual(now.AddMinutes(15), freeTime[1].StartTime);
            Assert.AreEqual(now.AddMinutes(20), freeTime[1].EndTime);

            Assert.AreEqual(now.AddMinutes(25), freeTime[2].StartTime);
            Assert.AreEqual(now.AddMinutes(30), freeTime[2].EndTime);

            Assert.AreEqual(now.AddMinutes(35), freeTime[3].StartTime);
            Assert.AreEqual(now.AddMinutes(40), freeTime[3].EndTime);

            Assert.AreEqual(now.AddMinutes(45), freeTime[4].StartTime);
            Assert.AreEqual(now.AddMinutes(50), freeTime[4].EndTime);

            Assert.AreEqual(now.AddMinutes(55), freeTime[5].StartTime);
            Assert.AreEqual(now.AddMinutes(60), freeTime[5].EndTime);

            Assert.AreEqual(now.AddMinutes(65), freeTime[6].StartTime);
            Assert.AreEqual(now.Date.AddDays(1), freeTime[6].EndTime);
        }
    }
}
