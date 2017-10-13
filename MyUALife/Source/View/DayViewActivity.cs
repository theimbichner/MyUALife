using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MyUALife
{
    [Activity(Label = "MyUALife")]
    public class DayViewActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.DayView);

            // Get the date from Intent, unpack the array, convert to DateTime
            int[] dateInfo = Intent.GetIntArrayExtra("date");
            int day = dateInfo[0];
            int month = dateInfo[1];
            int year = dateInfo[2];
            DateTime date = new DateTime(year, month, day); // Midnight of the given day

            // Create a range of DateTimes
            // We want to count midnight as belonging to the previous day.
            // Hence, we start our range just after midnight
            DateTime start = date.AddMilliseconds(1);
            DateTime end = date.AddDays(1);

            // Get the events in range from the calendar
            var events = Model.getCalendar().GetEventsInRange(start, end);

            // Get components by id
            LinearLayout eventsLayout = FindViewById<LinearLayout>(Resource.Id.eventsLayout);
            TextView dayLabel = FindViewById<TextView>(Resource.Id.dayLabel);
            Button backButton = FindViewById<Button>(Resource.Id.backButton);
            Button addEventButton = FindViewById<Button>(Resource.Id.addEventButton);

            // Add a button to the events layout for each event
            foreach (Event e in events)
            {
                Button button = new Button(this);
                button.Text = e.Name;
                eventsLayout.AddView(button);
            }

            // Setup the back button to take us back to the main activity
            backButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };

            // Setup the add event button to take us to the add event activity
            addEventButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(EventViewActivity));
                StartActivity(intent);
            };

            // Set the text on the label to indicate the date
            dayLabel.Text = "Events for " + month + "/" + day + "/" + year;
        }
    }
}