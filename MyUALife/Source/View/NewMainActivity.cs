using Android.App;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Android.Content;
using System;
using System.Collections.Generic;

namespace MyUALife
{
    [Activity(Label = "MyUALife", MainLauncher = true)]
    public class NewMainActivity : Activity
    {
        private DateTime loadedDate;
        private List<Event> loadedEvents;

        // GUI components
        private Button filterButton;
        private Button calendarButton;
        private Button happeningsButton;
        private Button createEventButton;
        private Button createDeadlineButton;
        private LinearLayout mainTextLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.NewMain);

            // Get components by id
            filterButton = FindViewById<Button>(Resource.Id.filterButton);
            calendarButton = FindViewById<Button>(Resource.Id.calendarButton);
            happeningsButton = FindViewById<Button>(Resource.Id.happeningsButton);
            createEventButton = FindViewById<Button>(Resource.Id.createEventButton);
            createDeadlineButton = FindViewById<Button>(Resource.Id.createDeadlineButton);
            mainTextLayout = FindViewById<LinearLayout>(Resource.Id.mainTextLayout);

            // Load the events scheduled for today
            LoadEvents();

            // Setup the filter button to filter events
            filterButton.Click += (sender, e) =>
            {

            };

            // Setup the calendar button to open the android calendar
            calendarButton.Click += (sender, e) =>
            {
                Android.Net.Uri.Builder builder = CalendarContract.ContentUri.BuildUpon();
                builder.AppendPath("time");
                long millis = (long)(DateTime.Today - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                ContentUris.AppendId(builder, millis);
                Intent intent = new Intent(Intent.ActionView).SetData(builder.Build());
                StartActivity(intent);
            };

            // Setup the happenings button to display a list of happenings
            happeningsButton.Click += (sender, e) =>
            {

            };

            // Setup the create event button to open the create event screen
            createEventButton.Click += (sender, e) =>
            {

            };

            // Setup the deadline button to open the create deadline screen
            createDeadlineButton.Click += (sender, e) =>
            {

            };
        }

        /*
         * Loads every event scheduled for the current day into the main text
         * view.
         */
        private void LoadEvents()
        {
            // Load events from today
            loadedDate = DateTime.Today;

            // Create a range of DateTimes
            // We want to count midnight as belonging to the previous day.
            // Hence, we start our range just after midnight
            DateTime start = loadedDate.AddMilliseconds(1);
            DateTime end = loadedDate.AddDays(1);

            // Get the events in range from the calendar
            loadedEvents = Model.getCalendar().GetEventsInRange(start, end);

            // Clear the main layout of text views
            mainTextLayout.RemoveAllViews();

            // Add a new text view for every event
            foreach (Event e in loadedEvents)
            {
                mainTextLayout.AddView(GenerateTextView(e));
            }
        }

        /*
         * Returns a new TextView that displays data about the given event.
         * When the returned TextView is long clicked, an AlertDialog is opened
         * asking the user if they want to edit the event.
         */
        private TextView GenerateTextView(Event calEvent)
        {
            // Create the text view and set its text
            TextView view = new TextView(this);
            view.Text = Description(calEvent);

            // Register the event handler to edit the event
            view.LongClick += (sender, e) =>
            {
                var infoDialog = new AlertDialog.Builder(this);
                infoDialog.SetMessage("Edit this event?");
                infoDialog.SetPositiveButton("Edit", delegate
                {
                    // Inform the user that this feature is not implemented
                    var notYetImplementedDialog = new AlertDialog.Builder(this);
                    notYetImplementedDialog.SetMessage("Not yet implemented");
                    notYetImplementedDialog.SetPositiveButton("Ok", delegate { });
                    notYetImplementedDialog.Show();
                });
                infoDialog.SetNegativeButton("Cancel", delegate { });
                infoDialog.Show();
            };
            return view;
        }

        private string Description(Event calEvent)
        {
            string format = "Name: {0}\nDescription: {1}\nFrom: {2}\nTo: {3}";
            object[] args = { calEvent.Name, calEvent.Description, calEvent.StartTime, calEvent.EndTime };
            return String.Format(format, args);
        }
    }
}

