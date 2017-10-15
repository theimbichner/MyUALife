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
        // GUI components
        private LinearLayout eventsLayout;

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
            eventsLayout = FindViewById<LinearLayout>(Resource.Id.eventsLayout);
            TextView dayLabel = FindViewById<TextView>(Resource.Id.dayLabel);
            Button backButton = FindViewById<Button>(Resource.Id.backButton);

            // Add a button to the events layout for each event
            foreach (Event e in events)
            {
                Button button = createDisplayButton(e);
                eventsLayout.AddView(button);
            }

            // Setup the back button to take us back to the main activity
            backButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };

            // Set the text on the label to indicate the date
            dayLabel.Text = "Events for " + month + "/" + day + "/" + year;
        }

        private Button createDisplayButton(Event calEvent)
        {
            Button button = new Button(this);
            button.Text = calEvent.Name;
            button.Click += (sender, e) =>
            {
                var infoDialog = new AlertDialog.Builder(this);
                string format = "Name: {0}\nDescription: {1}\nFrom: {2}\nTo: {3}";
                object[] args = {calEvent.Name, calEvent.Description, calEvent.StartTime, calEvent.EndTime};
                infoDialog.SetMessage(String.Format(format, args));
                infoDialog.SetPositiveButton("Ok", delegate { });
                infoDialog.SetNegativeButton("Edit", delegate
                {
                    var notYetImplementedDialog = new AlertDialog.Builder(this);
                    notYetImplementedDialog.SetMessage("Not yet implemented");
                    notYetImplementedDialog.SetPositiveButton("Ok", delegate { });
                    notYetImplementedDialog.Show();
                });
                infoDialog.Show();
            };
            button.LongClick += (sender, e) =>
            {
                var infoDialog = new AlertDialog.Builder(this);
                infoDialog.SetMessage("Delete this event?");
                infoDialog.SetNegativeButton("Cancel", delegate { });
                infoDialog.SetPositiveButton("Delete", delegate {
                    eventsLayout.RemoveView(button);
                    Model.getCalendar().RemoveEvent(calEvent);
                });
                infoDialog.Show();
            };
            return button;
        }
    }
}