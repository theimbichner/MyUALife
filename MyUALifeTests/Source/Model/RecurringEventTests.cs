using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using MyUALife;

namespace MyUALifeTests
{
    [TestClass()]
    public class RecurringEventTests
    {
        [TestMethod()]
        public void TestRecurringEvents()
        {
            DateTime now = DateTime.Now;
            Event e = new Event("name", "desc", Category.Appointment, now, now.AddMinutes(30));
            TimeSpan span = new TimeSpan(1, 20, 0);
            var gen = new RecurringEventGenerator(e, span);

            var list = gen.GenerateEvents(now.AddMinutes(120));
            Assert.AreEqual(1, list.Count);
            Event e2 = list[0];
            Assert.AreEqual(e.Name, e2.Name);
            Assert.AreEqual(e.Description, e2.Description);
            Assert.AreEqual(e.Type, e2.Type);
            Assert.AreEqual(now.AddMinutes(80), e2.StartTime);
            Assert.AreEqual(now.AddMinutes(110), e2.EndTime);

            list = gen.GenerateEvents(now.AddMinutes(190));
            Assert.AreEqual(1, list.Count);
            e2 = list[0];
            Assert.AreEqual(e.Name, e2.Name);
            Assert.AreEqual(e.Description, e2.Description);
            Assert.AreEqual(e.Type, e2.Type);
            Assert.AreEqual(now.AddMinutes(160), e2.StartTime);
            Assert.AreEqual(now.AddMinutes(190), e2.EndTime);

            list = gen.GenerateEvents(now.AddMinutes(255));
            Assert.AreEqual(1, list.Count);
            e2 = list[0];
            Assert.AreEqual(e.Name, e2.Name);
            Assert.AreEqual(e.Description, e2.Description);
            Assert.AreEqual(e.Type, e2.Type);
            Assert.AreEqual(now.AddMinutes(240), e2.StartTime);
            Assert.AreEqual(now.AddMinutes(270), e2.EndTime);

            list = gen.GenerateEvents(now.AddMinutes(320));
            Assert.AreEqual(1, list.Count);
            e2 = list[0];
            Assert.AreEqual(e.Name, e2.Name);
            Assert.AreEqual(e.Description, e2.Description);
            Assert.AreEqual(e.Type, e2.Type);
            Assert.AreEqual(now.AddMinutes(320), e2.StartTime);
            Assert.AreEqual(now.AddMinutes(350), e2.EndTime);
        }

        [TestMethod()]
        public void TestRecurringEventsEmpty()
        {
            DateTime now = DateTime.Now;
            Event e = new Event("name", "desc", Category.Appointment, now, now.AddMinutes(30));
            TimeSpan span = new TimeSpan(1, 20, 0);
            var gen = new RecurringEventGenerator(e, span);

            var list0 = gen.GenerateEvents(now);
            var list1 = gen.GenerateEvents(now.AddMinutes(-10));
            var list2 = gen.GenerateEvents(now);
            var list3 = gen.GenerateEvents(now.AddMinutes(29));
            var list4 = gen.GenerateEvents(now.AddMinutes(75));

            gen.GenerateEvents(now.AddHours(6));

            var list5 = gen.GenerateEvents(now.AddMinutes(90));
            var list6 = gen.GenerateEvents(now.AddMinutes(175));
            var list7 = gen.GenerateEvents(now.AddMinutes(330));
            var list8 = gen.GenerateEvents(now.AddMinutes(244));
            var list9 = gen.GenerateEvents(now.AddMinutes(399));

            Assert.AreEqual(0, list0.Count);
            Assert.AreEqual(0, list1.Count);
            Assert.AreEqual(0, list2.Count);
            Assert.AreEqual(0, list3.Count);
            Assert.AreEqual(0, list4.Count);
            Assert.AreEqual(0, list5.Count);
            Assert.AreEqual(0, list6.Count);
            Assert.AreEqual(0, list7.Count);
            Assert.AreEqual(0, list8.Count);
            Assert.AreEqual(0, list9.Count);
        }

        [TestMethod()]
        public void TestRecurringEventsBatch()
        {
            DateTime now = DateTime.Now;
            Event e = new Event("name", "desc", Category.Appointment, now, now.AddMinutes(30));
            TimeSpan span = new TimeSpan(1, 20, 0);
            var gen = new RecurringEventGenerator(e, span);

            var list = gen.GenerateEvents(now.AddMinutes(800));
            Assert.AreEqual(10, list.Count);
            for (int i = 1; i <= 10; i++)
            {
                Event e2 = list[i - 1];
                Assert.AreEqual(now.AddMinutes(80 * i), e2.StartTime);
                Assert.AreEqual(now.AddMinutes(80 * i + 30), e2.EndTime);
            }
        }
    }
}
