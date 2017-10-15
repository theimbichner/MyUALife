using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;

namespace MyUALife
{
    [Activity(Label = "MyUALife", MainLauncher = true)]
    public class MainActivity : Activity, CalendarView.IOnDateChangeListener
    {
        private int selectedDay = DateTime.Now.Day;
        private int selectedMonth = DateTime.Now.Month;
        private int selectedYear = DateTime.Now.Year;

        // GUI components
        Button openDayViewButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.Main);

            // Get components by id
            CalendarView mainCalendar = FindViewById<CalendarView>(Resource.Id.mainCalendar);
            openDayViewButton = FindViewById<Button>(Resource.Id.openDayViewButton);
            Button addEventButton = FindViewById<Button>(Resource.Id.addEventButton);

            // Create dateFetcher to read the current date from the calendar display
            mainCalendar.SetOnDateChangeListener(this);

            // Setup the button to open a new view when clicked.
            openDayViewButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(DayViewActivity));
                intent.PutExtra("date", new int[] {selectedDay, selectedMonth, selectedYear});
                StartActivity(intent);
            };

            // Initialize the text for the button
            updateButtonText();

            // Setup the add event button to open the new event view when clicked
            addEventButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(EventViewActivity));
                StartActivity(intent);
            };
        }

        public void OnSelectedDayChange(CalendarView view, int year, int month, int day)
        {
            selectedDay = day;
            selectedYear = year;
            selectedMonth = month + 1; // convert from 0-start to 1-start
            updateButtonText();
        }

        private void updateButtonText()
        {
            openDayViewButton.Text = String.Format("View events for {0}/{1}/{2}", selectedMonth, selectedDay, selectedYear);
        }
    }
}

