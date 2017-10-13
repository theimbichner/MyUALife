using Android.App;
using Android.Widget;
using Android.OS;
using System;

namespace MyUALife
{
    /*
     * Retrieves the current date selected by a given CalendarView at any time.
     * Any values retrieved through this class are 1-start.
     */
    public class CalendarDateFetcher : Java.Lang.Object, CalendarView.IOnDateChangeListener
    {
        // The current day of the month
        private int day;

        // The current month
        private int month;

        // The current year
        private int year;

        /*
         * Creates a CalendarDateFetcher that fetches the date corresponding to
         * the supplied CalendarView.
         */
        public CalendarDateFetcher(CalendarView cal)
        {
            // Initialize fields to correspond to the current date
            DateTime date = DateTime.Now;
            day = date.Day;
            month = date.Month;
            year = date.Year;

            // Register as a listener for date changes
            cal.SetOnDateChangeListener(this);
        }

        /*
         * Listener function for CalendarView. This method is called whenever the CalendarView associated
         * with this changes its selected date.
         */
        public void OnSelectedDayChange(CalendarView cal, int year, int month, int day)
        {
            this.day = day;
            this.year = year;
            this.month = month + 1; // convert from 0-start to 1-start
        }

        /*
         * Returns an array of three ints that specify the current date. Values
         * are stored in the order: day, month, year.
         */
        public int[] getDate()
        {
            return new int[] {day, month, year};
        }
    }
}