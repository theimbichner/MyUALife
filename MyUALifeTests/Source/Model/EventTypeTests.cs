using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyUALife;

namespace MyUALifeTests
{
    [TestClass()]
    public class EventTypeTests
    {
        [TestMethod()]
        public void TestEventType()
        {
            EventType type = new EventType("name", 4, false, "color");

            Assert.AreEqual("name", type.Name);
            Assert.AreEqual(4, type.Priority);
            Assert.IsFalse(type.IsEditable);
            Assert.AreEqual("color", type.ColorString);
        }

        [TestMethod()]
        public void TestEventTypeGetByName()
        {
            var type = Category.GetTypeByName("Homework");
            Assert.AreEqual(Category.Homework, type);

            type = Category.GetTypeByName("Class Time");
            Assert.AreEqual(Category.ClassTime, type);

            type = Category.GetTypeByName("Absent");
            Assert.AreEqual(Category.FreeTime, type);
        }

        [TestMethod()]
        public void TestFilterSets()
        {
            var filters = FilterSet.FilterSets;
            FilterSet filterSet = filters.Find(fs => fs.Name == "All types");
            Assert.IsNotNull(filterSet);
            foreach (EventType t in Category.CreatableTypes)
            {
                Assert.IsTrue(filterSet.AllowedTypes.Contains(t));
            }

            filterSet = filters.Find(fs => fs.Name == "Recreation");
            Assert.IsNotNull(filterSet);
            Assert.IsTrue(filterSet.AllowedTypes.Contains(Category.Recreation));
            Assert.IsTrue(filterSet.AllowedTypes.Count == 1);

            filterSet = filters.Find(fs => fs.Name == "Academics");
            Assert.IsNotNull(filterSet);
            Assert.IsTrue(filterSet.AllowedTypes.Contains(Category.ClassTime));
            Assert.IsTrue(filterSet.AllowedTypes.Contains(Category.StudyTime));
            Assert.IsTrue(filterSet.AllowedTypes.Contains(Category.Homework));
            Assert.IsTrue(filterSet.AllowedTypes.Count == 3);
        }
    }
}
