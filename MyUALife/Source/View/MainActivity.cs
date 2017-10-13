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
        private CalendarDateFetcher dateFetcher;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.Main);

            // Create dateFetcher to read the current date from the calendar display
            CalendarView mainCalendar = FindViewById<CalendarView>(Resource.Id.mainCalendar);
            dateFetcher = new CalendarDateFetcher(mainCalendar);

            // Setup the button to open a new view when clicked.
            Button openDayViewButton = FindViewById<Button>(Resource.Id.openDayViewButton);
            openDayViewButton.Click += onClick;
        }

        /*
         * Opens a new activity that displays events occuring on the currently selected day.
         */
        private void onClick(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(DayViewActivity));
            intent.PutExtra("date", dateFetcher.getDate());
            StartActivity(intent);
        }
    }
}

