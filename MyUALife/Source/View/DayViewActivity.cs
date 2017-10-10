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
    [Activity(Label = "Today's Events")]
    public class DayViewActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.DayView);

            // Get the date from Intent and unpack the array
            int[] date = Intent.GetIntArrayExtra("date");
            int day = date[0];
            int month = date[1];
            int year = date[2];

            // Convert into a DateTime range
            DateTime start = new DateTime(year, month, day); // Midnight today
            DateTime end = new DateTime(year, month, day + 1); // Midnight tomorrow

            // Get the events in range from the calendar
            var events = Model.getCalendar().GetEventsInRange(start, end);

            // Get the main layout and label by id
            LinearLayout mainLayout = FindViewById<LinearLayout>(Resource.Id.mainLayout);
            TextView dayLabel = FindViewById<TextView>(Resource.Id.dayLabel);

            // Set the text on the label to indicate the date
            dayLabel.Text = "Events for " + month + "/" + day + "/" + year;

            // Add a button for each event
            foreach (Event e in events)
            {
                Button button = new Button(this);
                button.Text = e.Name;
                mainLayout.AddView(button);
            }

        }
    }
}