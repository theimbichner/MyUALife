using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;

namespace MyUALife
{
    [Activity(Label = "MyUALife", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private DateManager dateManager = new DateManager();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Set the calendar to inform dateManager whenever the date changes
            CalendarView mainCalendar = FindViewById<CalendarView>(Resource.Id.mainCalendar);
            dateManager.initDay(mainCalendar.Date);
            mainCalendar.SetOnDateChangeListener(dateManager);

            // Show a message when the button is clicked
            Button openDayViewButton = FindViewById<Button>(Resource.Id.openDayViewButton);
            openDayViewButton.Click += onClick;
        }

        /*
         * Displays an AlertDialog box that informs the user of the current date selected on the main calendar.
         */
        protected void onClick(object sender, EventArgs e)
        {
            var infoDialog = new AlertDialog.Builder(this);
            infoDialog.SetMessage(dateManager.getMessage());
            infoDialog.SetNeutralButton("Ok", delegate { });
            infoDialog.Show();
        }
    }

    public class DateManager : Java.Lang.Object, CalendarView.IOnDateChangeListener
    {
        // The current day of the month
        private int day;

        // The current month, starting from 0 
        private int month;

        // The current year
        private int year;

        /*
         * Listener function for CalendarView. This method is called whenever the CalendarView associated
         * with this manager changes its selected date.
         */
        public void OnSelectedDayChange(CalendarView cal, int year, int month, int day)
        {
            this.day = day;
            this.year = year;
            this.month = month;
        }

        /*
         * Gives a simple message informing the user of the current date. 
         */
        public string getMessage()
        {
            return "day: " + day + ", month: " + (month + 1) + ", year: " + year;
        }

        public void initDay(long dateMillis)
        {
            DateTime date = new DateTime(dateMillis * TimeSpan.TicksPerMillisecond);
            day = date.Day - 1;
            month = date.Month - 1;
            year = date.Year + 1969;
        }
    }
}

