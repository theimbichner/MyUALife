using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass()]
    public class CalendarEventTests
    {
        [TestMethod()]
        public void GetEventsInRangeTest1()
        {
            DateTime basis = DateTime.Now;
            List<Event> events = new List<Event>();

            Event event1 = new Event("Event1", "desc1", Category.GetTypeByName("Homework"), basis.AddMinutes(10), basis.AddMinutes(15));
            Event event2 = new Event("Event2", "desc2", Category.GetTypeByName("Homework"), basis.AddMinutes(20), basis.AddMinutes(25));
            Event event3 = new Event("Event3", "desc3", Category.GetTypeByName("Homework"), basis.AddMinutes(30), basis.AddMinutes(35));
            Event event4 = new Event("Event4", "desc4", Category.GetTypeByName("Homework"), basis.AddMinutes(40), basis.AddMinutes(45));
            Event event5 = new Event("Event5", "desc5", Category.GetTypeByName("Homework"), basis.AddMinutes(50), basis.AddMinutes(55));
            Event event6 = new Event("Event6", "desc6", Category.GetTypeByName("Homework"), basis.AddMinutes(60), basis.AddMinutes(65));

            events.Add(event1);
            events.Add(event2);
            events.Add(event3);
            events.Add(event4);
            events.Add(event5);
            events.Add(event6);

            Calendar theCalendar = new Calendar(events, null);

            List<Event> temp = new List<Event>();
            temp = theCalendar.GetEventsInRange(basis.AddMinutes(70), basis.AddMinutes(75));
            Assert.IsFalse(temp.Contains(event1));
            Assert.IsTrue(temp.Contains(event2));
            Assert.IsTrue(temp.Contains(event3));
            Assert.IsFalse(temp.Contains(event4));
            Assert.IsFalse(temp.Contains(event5));
            Assert.IsFalse(temp.Contains(event6));
        }

        [TestMethod()]
        public void GetEventsInRange2()
        {
            DateTime basis = DateTime.Now;
            List<Event> events = new List<Event>();

            Event event1 = new Event("Event1", "desc1", Category.GetTypeByName("Homework"), basis.AddMinutes(10), basis.AddMinutes(15));
            Event event2 = new Event("Event2", "desc2", Category.GetTypeByName("Homework"), basis.AddMinutes(20), basis.AddMinutes(25));
            Event event3 = new Event("Event3", "desc3", Category.GetTypeByName("Homework"), basis.AddMinutes(30), basis.AddMinutes(35));
            Event event4 = new Event("Event4", "desc4", Category.GetTypeByName("Homework"), basis.AddMinutes(40), basis.AddMinutes(45));
            Event event5 = new Event("Event5", "desc5", Category.GetTypeByName("Homework"), basis.AddMinutes(50), basis.AddMinutes(55));
            Event event6 = new Event("Event6", "desc6", Category.GetTypeByName("Homework"), basis.AddMinutes(60), basis.AddMinutes(65));

            events.Add(event1);
            events.Add(event2);
            events.Add(event3);
            events.Add(event4);
            events.Add(event5);
            events.Add(event6);

            Calendar theCalendar = new Calendar(events, null);
            List<Event> temp = new List<Event>();
            temp = theCalendar.GetEventsInRange(basis.AddMinutes(0), basis.AddMinutes(11));
            Assert.IsTrue(temp.Contains(event1));
            Assert.IsFalse(temp.Contains(event2));
            Assert.IsFalse(temp.Contains(event3));
            Assert.IsFalse(temp.Contains(event4));
            Assert.IsFalse(temp.Contains(event5));
            Assert.IsFalse(temp.Contains(event6));
        }

        [TestMethod()]
        public void GetEventsInRange3()
        {
            DateTime basis = DateTime.Now;
            List<Event> events = new List<Event>();

            Event event1 = new Event("Event1", "desc1", Category.GetTypeByName("Homework"), basis.AddMinutes(10), basis.AddMinutes(15));
            Event event2 = new Event("Event2", "desc2", Category.GetTypeByName("Homework"), basis.AddMinutes(20), basis.AddMinutes(25));
            Event event3 = new Event("Event3", "desc3", Category.GetTypeByName("Homework"), basis.AddMinutes(30), basis.AddMinutes(35));
            Event event4 = new Event("Event4", "desc4", Category.GetTypeByName("Homework"), basis.AddMinutes(40), basis.AddMinutes(45));
            Event event5 = new Event("Event5", "desc5", Category.GetTypeByName("Homework"), basis.AddMinutes(50), basis.AddMinutes(55));
            Event event6 = new Event("Event6", "desc6", Category.GetTypeByName("Homework"), basis.AddMinutes(60), basis.AddMinutes(65));

            events.Add(event1);
            events.Add(event2);
            events.Add(event3);
            events.Add(event4);
            events.Add(event5);
            events.Add(event6);

            Calendar theCalendar = new Calendar(events, null);
            List<Event> temp = new List<Event>();
            temp = theCalendar.GetEventsInRange(basis.AddMinutes(16), basis.AddMinutes(21));
            Assert.IsFalse(temp.Contains(event1));
            Assert.IsTrue(temp.Contains(event2));
            Assert.IsFalse(temp.Contains(event3));
            Assert.IsFalse(temp.Contains(event4));
            Assert.IsFalse(temp.Contains(event5));
            Assert.IsFalse(temp.Contains(event6));
        }
    }
}