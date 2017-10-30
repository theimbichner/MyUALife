using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests {
	[TestClass()]
	public class CalendarTests {

		[TestMethod()]
		public void AddEventTest() {
			Calendar c = new Calendar();
			DateTime start = new DateTime(2009, 4, 13, 4, 13, 0);
			DateTime end = new DateTime(2009, 4, 13, 6, 12, 0);
			Event e = new Event("HI!", "description", Category.other, start, end);
			c.AddEvent(e);
			List<Event> events = c.GetEventsInRange(start.AddDays(-1), end.AddDays(1));
			Assert.AreEqual(events.Count, 1);
			Event e2 = events[0];
			Assert.AreEqual(e, e2);
		}

		[TestMethod()]
		public void GetDeadlinesAfterTimeTest() {
			Calendar c = new Calendar();
			DateTime start = new DateTime(2009, 4, 13, 0, 0, 0);

			Deadline d1 = new Deadline("d", "", new DateTime(2009, 4, 13, 4, 13, 0));
			c.AddDeadline(d1);
			Assert.AreEqual(c.GetDeadlinesAfterTime(start).Count, 1);

			Deadline d2 = new Deadline("d", "", new DateTime(2009, 4, 13, 4, 13, 0));
			c.AddDeadline(d2);
			Assert.AreEqual(c.GetDeadlinesAfterTime(start).Count, 2);

			Deadline d3 = new Deadline("d", "", new DateTime(2009, 4, 13, 6, 12, 0));
			c.AddDeadline(d3);
			Assert.AreEqual(c.GetDeadlinesAfterTime(start).Count, 3);

			Deadline d4 = new Deadline("d", "", new DateTime(2009, 4, 12, 6, 12, 0));
			c.AddDeadline(d4);
			Assert.AreEqual(c.GetDeadlinesAfterTime(start).Count, 3);
		}

		[TestMethod()]
		public void GetFreeTimeBlocksInRangeTest() {

			DateTime start = new DateTime(2009, 4, 13, 0, 0, 0);
			DateTime end = new DateTime(2009, 4, 13, 23, 30, 0);

			TestThreeFreeTimes(
				start, end,

				new DateTime(2009, 4, 13, 4, 0, 0),
				new DateTime(2009, 4, 13, 4, 30, 0),

				new DateTime(2009, 4, 13, 5, 0, 0),
				new DateTime(2009, 4, 13, 6, 0, 0)
			);

			TestThreeFreeTimes(
				start, end,

				new DateTime(2009, 4, 13, 4, 0, 0),
				new DateTime(2009, 4, 13, 4, 59, 0),

				new DateTime(2009, 4, 13, 5, 0, 0),
				new DateTime(2009, 4, 13, 6, 0, 0)
			);

			TestThreeFreeTimes(
				start, end,

				new DateTime(2009, 4, 13, 4, 0, 0),
				new DateTime(2009, 4, 13, 4, 0, 0),

				new DateTime(2009, 4, 13, 5, 0, 0),
				new DateTime(2009, 4, 13, 6, 0, 0)
			);

		}

		public void TestThreeFreeTimes(DateTime start, DateTime end, DateTime e1Start, DateTime e1End, DateTime e2Start, DateTime e2End) {
			Calendar c = new Calendar();

			Event e1 = new Event("E1", "", Category.other, e1Start, e1End);
			Event e2 = new Event("E2", "", Category.other, e2Start, e2End);

			c.AddEvent(e1); c.AddEvent(e2);

			List<Event> freeBlocks = c.GetFreeTimeBlocksInRange(start, end);

			Assert.AreEqual(freeBlocks.Count, 3);
			Assert.AreEqual(freeBlocks[0].StartTime, start);
			Assert.AreEqual(freeBlocks[0].EndTime, e1Start);
			Assert.AreEqual(freeBlocks[1].StartTime, e1End);
			Assert.AreEqual(freeBlocks[1].EndTime, e2Start);
			Assert.AreEqual(freeBlocks[2].StartTime, e2End);
			Assert.AreEqual(freeBlocks[2].EndTime, end);
		}

	}
}