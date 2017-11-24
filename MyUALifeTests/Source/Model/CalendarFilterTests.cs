using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using MyUALife;

namespace MyUALifeTests
{
    [TestClass()]
    public class CalendarFilterTests
    {
        [TestMethod()]
        public void FilterTest1()
        {
            DateTime basis = DateTime.Now;
            List<Event> events = new List<Event>();

            Event event1 = new Event("Event1", "desc1", Category.GetTypeByName("Homework"), basis.AddMinutes(10), basis.AddMinutes(15));
            Event event2 = new Event("Event2", "desc2", Category.GetTypeByName("StudyTime"), basis.AddMinutes(20), basis.AddMinutes(25));
            Event event3 = new Event("Event3", "desc3", Category.GetTypeByName("ClassTime"), basis.AddMinutes(30), basis.AddMinutes(35));
            Event event4 = new Event("Event4", "desc4", Category.GetTypeByName("Appointment"), basis.AddMinutes(40), basis.AddMinutes(45));
            Event event5 = new Event("Event5", "desc5", Category.GetTypeByName("Recreation"), basis.AddMinutes(50), basis.AddMinutes(55));
            Event event6 = new Event("Event6", "desc6", Category.GetTypeByName("Homework"), basis.AddMinutes(60), basis.AddMinutes(65));

            events.Add(event1);
            events.Add(event2);
            events.Add(event3);
            events.Add(event4);
            events.Add(event5);
            events.Add(event6);
            
            List<Event> temp = new List<Event>();
            temp = Calendar.FilterEventsByType(events, Category.GetTypeByName("Homework"));

            Assert.IsTrue(temp.Contains(event1));
            Assert.IsFalse(temp.Contains(event2));
            Assert.IsFalse(temp.Contains(event3));
            Assert.IsFalse(temp.Contains(event4));
            Assert.IsFalse(temp.Contains(event5));
            Assert.IsTrue(temp.Contains(event6));
        }

        [TestMethod()]
        public void FilterTest2()
        {
            DateTime basis = DateTime.Now;
            List<Event> events = new List<Event>();

            Event event1 = new Event("Event1", "desc1", Category.GetTypeByName("Homework"), basis.AddMinutes(10), basis.AddMinutes(15));
            Event event2 = new Event("Event2", "desc2", Category.GetTypeByName("StudyTime"), basis.AddMinutes(20), basis.AddMinutes(25));
            Event event3 = new Event("Event3", "desc3", Category.GetTypeByName("ClassTime"), basis.AddMinutes(30), basis.AddMinutes(35));
            Event event4 = new Event("Event4", "desc4", Category.GetTypeByName("Appointment"), basis.AddMinutes(40), basis.AddMinutes(45));
            Event event5 = new Event("Event5", "desc5", Category.GetTypeByName("Recreation"), basis.AddMinutes(50), basis.AddMinutes(55));
            Event event6 = new Event("Event6", "desc6", Category.GetTypeByName("Homework"), basis.AddMinutes(60), basis.AddMinutes(65));

            events.Add(event1);
            events.Add(event2);
            events.Add(event3);
            events.Add(event4);
            events.Add(event5);
            events.Add(event6);
            
            List<Event> temp = new List<Event>();
            temp = Calendar.FilterEventsByType(events, Category.GetTypeByName("StudyTime"));

            Assert.IsFalse(temp.Contains(event1));
            Assert.IsTrue(temp.Contains(event2));
            Assert.IsFalse(temp.Contains(event3));
            Assert.IsFalse(temp.Contains(event4));
            Assert.IsFalse(temp.Contains(event5));
            Assert.IsFalse(temp.Contains(event6));
        }

        [TestMethod()]
        public void FilterTest3()
        {
            DateTime basis = DateTime.Now;
            List<Event> events = new List<Event>();

            Event event1 = new Event("Event1", "desc1", Category.GetTypeByName("Homework"), basis.AddMinutes(10), basis.AddMinutes(15));
            Event event2 = new Event("Event2", "desc2", Category.GetTypeByName("StudyTime"), basis.AddMinutes(20), basis.AddMinutes(25));
            Event event3 = new Event("Event3", "desc3", Category.GetTypeByName("ClassTime"), basis.AddMinutes(30), basis.AddMinutes(35));
            Event event4 = new Event("Event4", "desc4", Category.GetTypeByName("Appointment"), basis.AddMinutes(40), basis.AddMinutes(45));
            Event event5 = new Event("Event5", "desc5", Category.GetTypeByName("Recreation"), basis.AddMinutes(50), basis.AddMinutes(55));
            Event event6 = new Event("Event6", "desc6", Category.GetTypeByName("Homework"), basis.AddMinutes(60), basis.AddMinutes(65));

            events.Add(event1);
            events.Add(event2);
            events.Add(event3);
            events.Add(event4);
            events.Add(event5);
            events.Add(event6);

            List<Event> temp = new List<Event>();
            temp = Calendar.FilterEventsByType(events, Category.GetTypeByName("ClassTime"));

            Assert.IsFalse(temp.Contains(event1));
            Assert.IsFalse(temp.Contains(event2));
            Assert.IsTrue(temp.Contains(event3));
            Assert.IsFalse(temp.Contains(event4));
            Assert.IsFalse(temp.Contains(event5));
            Assert.IsFalse(temp.Contains(event6));
        }

        [TestMethod()]
        public void FilterTest4()
        {
            DateTime basis = DateTime.Now;
            List<Event> events = new List<Event>();

            Event event1 = new Event("Event1", "desc1", Category.GetTypeByName("Homework"), basis.AddMinutes(10), basis.AddMinutes(15));
            Event event2 = new Event("Event2", "desc2", Category.GetTypeByName("StudyTime"), basis.AddMinutes(20), basis.AddMinutes(25));
            Event event3 = new Event("Event3", "desc3", Category.GetTypeByName("ClassTime"), basis.AddMinutes(30), basis.AddMinutes(35));
            Event event4 = new Event("Event4", "desc4", Category.GetTypeByName("Appointment"), basis.AddMinutes(40), basis.AddMinutes(45));
            Event event5 = new Event("Event5", "desc5", Category.GetTypeByName("Recreation"), basis.AddMinutes(50), basis.AddMinutes(55));
            Event event6 = new Event("Event6", "desc6", Category.GetTypeByName("Homework"), basis.AddMinutes(60), basis.AddMinutes(65));

            events.Add(event1);
            events.Add(event2);
            events.Add(event3);
            events.Add(event4);
            events.Add(event5);
            events.Add(event6);

            List<Event> temp = new List<Event>();
            temp = Calendar.FilterEventsByType(events, Category.GetTypeByName("Appointment"));

            Assert.IsFalse(temp.Contains(event1));
            Assert.IsFalse(temp.Contains(event2));
            Assert.IsFalse(temp.Contains(event3));
            Assert.IsTrue(temp.Contains(event4));
            Assert.IsFalse(temp.Contains(event5));
            Assert.IsFalse(temp.Contains(event6));
        }

        [TestMethod()]
        public void FilterTest5()
        {
            DateTime basis = DateTime.Now;
            List<Event> events = new List<Event>();

            Event event1 = new Event("Event1", "desc1", Category.GetTypeByName("Homework"), basis.AddMinutes(10), basis.AddMinutes(15));
            Event event2 = new Event("Event2", "desc2", Category.GetTypeByName("StudyTime"), basis.AddMinutes(20), basis.AddMinutes(25));
            Event event3 = new Event("Event3", "desc3", Category.GetTypeByName("ClassTime"), basis.AddMinutes(30), basis.AddMinutes(35));
            Event event4 = new Event("Event4", "desc4", Category.GetTypeByName("Appointment"), basis.AddMinutes(40), basis.AddMinutes(45));
            Event event5 = new Event("Event5", "desc5", Category.GetTypeByName("Recreation"), basis.AddMinutes(50), basis.AddMinutes(55));
            Event event6 = new Event("Event6", "desc6", Category.GetTypeByName("Homework"), basis.AddMinutes(60), basis.AddMinutes(65));

            events.Add(event1);
            events.Add(event2);
            events.Add(event3);
            events.Add(event4);
            events.Add(event5);
            events.Add(event6);

            List<Event> temp = new List<Event>();
            temp = Calendar.FilterEventsByType(events, Category.GetTypeByName("Recreation"));

            Assert.IsFalse(temp.Contains(event1));
            Assert.IsFalse(temp.Contains(event2));
            Assert.IsFalse(temp.Contains(event3));
            Assert.IsFalse(temp.Contains(event4));
            Assert.IsTrue(temp.Contains(event5));
            Assert.IsFalse(temp.Contains(event6));
        }

        [TestMethod()]
        public void FilterTest6()
        {
            DateTime basis = DateTime.Now;
            List<Event> events = new List<Event>();

            Event event1 = new Event("Event1", "desc1", Category.GetTypeByName("Homework"), basis.AddMinutes(10), basis.AddMinutes(15));
            Event event2 = new Event("Event2", "desc2", Category.GetTypeByName("StudyTime"), basis.AddMinutes(20), basis.AddMinutes(25));
            Event event3 = new Event("Event3", "desc3", Category.GetTypeByName("ClassTime"), basis.AddMinutes(30), basis.AddMinutes(35));
            Event event4 = new Event("Event4", "desc4", Category.GetTypeByName("Appointment"), basis.AddMinutes(40), basis.AddMinutes(45));
            Event event5 = new Event("Event5", "desc5", Category.GetTypeByName("Recreation"), basis.AddMinutes(50), basis.AddMinutes(55));
            Event event6 = new Event("Event6", "desc6", Category.GetTypeByName("Homework"), basis.AddMinutes(60), basis.AddMinutes(65));

            events.Add(event1);
            events.Add(event2);
            events.Add(event3);
            events.Add(event4);
            events.Add(event5);
            events.Add(event6);

            List<Event> temp = new List<Event>();
            List<EventType> types = new List<EventType>();
            types.Add(Category.GetTypeByName("Homework"));
            types.Add(Category.GetTypeByName("ClassTime"));
            temp = Calendar.FilterEventsByTypes(events, types);

            Assert.IsTrue(temp.Contains(event1));
            Assert.IsFalse(temp.Contains(event2));
            Assert.IsTrue(temp.Contains(event3));
            Assert.IsFalse(temp.Contains(event4));
            Assert.IsFalse(temp.Contains(event5));
            Assert.IsTrue(temp.Contains(event6));
        }
    }
}