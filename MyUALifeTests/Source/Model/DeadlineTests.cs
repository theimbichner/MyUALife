using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyUALife;
using System;

namespace MyUALifeTests
{
    [TestClass()]
    public class DeadlineTests
    {
        [TestMethod()]
        public void TestDeadline()
        {
            DateTime now = DateTime.Now;
            Deadline deadline1 = new Deadline("name", "desc", now);
            Deadline deadline2 = new Deadline("name2", "desc2", now.AddSeconds(34), Category.Recreation);

            Assert.AreEqual("name", deadline1.Name);
            Assert.AreEqual("name2", deadline2.Name);
            Assert.AreEqual("desc", deadline1.Description);
            Assert.AreEqual("desc2", deadline2.Description);
            Assert.AreEqual(now, deadline1.Time);
            Assert.AreEqual(now.AddSeconds(34), deadline2.Time);
            Assert.AreEqual(Category.Other, deadline1.Type);
            Assert.AreEqual(Category.Recreation, deadline2.Type);
        }

        [TestMethod()]
        public void TestDeadlineCompareTo()
        {
            DateTime now = DateTime.Now;
            Deadline deadline1 = new Deadline("name", "desc", now);
            Deadline deadline2 = new Deadline("name2", "desc2", now.AddSeconds(34), Category.Recreation);
            Assert.IsTrue(deadline1.CompareTo(deadline2) < 0);
            Assert.IsTrue(deadline2.CompareTo(deadline1) > 0);
        }
    }
}
