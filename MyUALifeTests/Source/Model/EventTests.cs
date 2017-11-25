using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyUALife;
using System;

namespace MyUALifeTests
{
    [TestClass()]
    public class EventTests
    {
        [TestMethod()]
        public void TestEvent()
        {
            DateTime now = DateTime.Now;

            Event e = new Event("name", "desc", Category.Appointment, now, now.AddDays(20));
            Assert.AreEqual("name", e.Name);
            Assert.AreEqual("desc", e.Description);
            Assert.AreEqual(Category.Appointment, e.Type);
            Assert.AreEqual(now, e.StartTime);
            Assert.AreEqual(now.AddDays(20), e.EndTime);
        }

        [TestMethod()]
        public void TestEventShift()
        {
            DateTime now = DateTime.Now;
            Event e1 = new Event("name", "desc", Category.Appointment, now, now.AddDays(20));
            TimeSpan span = new TimeSpan(2, 3, 24, 59);
            Event e2 = e1.Shift(span);
            Assert.AreEqual(now.AddDays(2).AddHours(3).AddMinutes(24).AddSeconds(59), e2.StartTime);
            Assert.AreEqual(now.AddDays(22).AddHours(3).AddMinutes(24).AddSeconds(59), e2.EndTime);
        }

        [TestMethod()]
        public void TestEventCompareTo()
        {
            DateTime now = DateTime.Now;
            Event e1 = new Event("name", "desc", Category.Appointment, now,               now.AddMinutes(20));
            Event e2 = new Event("name", "desc", Category.Appointment, now.AddMinutes(5), now.AddMinutes(20));
            Event e3 = new Event("name", "desc", Category.Appointment, now,               now.AddMinutes(10));
            Event e4 = new Event("name", "desc", Category.Appointment, now.AddMinutes(5), now.AddMinutes(10));

            Assert.IsTrue(e1.CompareTo(e1) == 0);
            Assert.IsTrue(e1.CompareTo(e2) < 0);
            Assert.IsTrue(e1.CompareTo(e3) == 0);
            Assert.IsTrue(e1.CompareTo(e4) < 0);
            Assert.IsTrue(e2.CompareTo(e1) > 0);
            Assert.IsTrue(e2.CompareTo(e3) > 0);
            Assert.IsTrue(e3.CompareTo(e1) == 0);
            Assert.IsTrue(e3.CompareTo(e2) < 0);
            Assert.IsTrue(e4.CompareTo(e1) > 0);
        }
    }
}
