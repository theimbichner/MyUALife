using Android.App;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Android.Content;
using Android.Runtime;
using System;
using System.Collections.Generic;

namespace MyUALife
{
    [Activity(Label = "MyUALife", MainLauncher = true)]
    public class NewMainActivity : Activity
    {
        // Request codes for the EventEditorActivity
        private const int AddEventRequest = 1;
        private const int EditEventRequest = 2;

        // The date whose events are currently displayed
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
                StartAddEventActivity();
            };

            // Setup the deadline button to open the create deadline screen
            createDeadlineButton.Click += (sender, e) =>
            {

            };
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Load the events scheduled for today
            LoadEvents();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                // Add the resulting event to the calendar
                Event resultEvent = new EventSerializer(data).ReadEvent(EventSerializer.ResultEvent);
                Model.Calendar.AddEvent(resultEvent);
            }
            else
            {
                // The user selected to add an event, but backed out without saving
            }
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
            loadedEvents = Model.Calendar.GetEventsInRange(start, end);

            // Clear the layout and add a text view for every event
            ViewUtil util = new ViewUtil(this);
            util.LoadEventsToLayout(mainTextLayout, loadedEvents);
        }

        /*
         * Starts the EventEditorActivity. No event is passed to the editor, so
         * the returned event will be wholly new.
         */
        public void StartAddEventActivity()
        {
            Intent intent = new Intent(this, typeof(EventEditorActivity));
            StartActivityForResult(intent, AddEventRequest);
        }

        /*
         * Starts the editor with the given event's info as the starting value
         * of the editors components. The returned value will replace the given
         * event in the calendar.
         */
        public void StartEditEventActivity(Event calendarEvent)
        {
            Intent intent = new Intent(this, typeof(EventEditorActivity));
            new EventSerializer(intent).WriteEvent(EventSerializer.InputEvent, calendarEvent);
            Model.Calendar.RemoveEvent(calendarEvent);
            StartActivityForResult(intent, EditEventRequest);
        }
    }
}