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
    public class MainActivity : Activity
    {
        // Request codes for the EventEditorActivity
        private const int AddEventRequest = 1;
        private const int EditEventRequest = 2;

        // Request codes for the DeadlineEditorActivity
        private const int AddDeadlineRequest = 3;

        // The opened tab -- true: events tab, false: deadlines tab
        private bool eventsTabOpen = true;

        // GUI components
        private Button filterButton;
        private Button calendarButton;
        private Button happeningsButton;
        private Button createEventButton;
        private Button createDeadlineButton;
        private LinearLayout mainTextLayout;
        private RadioButton eventsTab;
        private RadioButton deadlinesTab;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.Main);

            // Get components by id
            filterButton = FindViewById<Button>(Resource.Id.filterButton);
            calendarButton = FindViewById<Button>(Resource.Id.calendarButton);
            happeningsButton = FindViewById<Button>(Resource.Id.happeningsButton);
            createEventButton = FindViewById<Button>(Resource.Id.createEventButton);
            createDeadlineButton = FindViewById<Button>(Resource.Id.createDeadlineButton);
            mainTextLayout = FindViewById<LinearLayout>(Resource.Id.mainTextLayout);
            eventsTab = FindViewById<RadioButton>(Resource.Id.eventsRadioButton);
            deadlinesTab = FindViewById<RadioButton>(Resource.Id.deadlinesRadioButton);

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
                StartAddDeadlineActivity();
            };

            // Setup the events tab to display the events list
            eventsTab.Click += (sender, e) =>
            {
                LoadEvents();
                eventsTabOpen = true;
            };

            // Setup the deadlines tab to display the deadlines list
            deadlinesTab.Click += (sender, e) =>
            {
                LoadDeadlines();
                eventsTabOpen = false;
            };
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Load the events scheduled for today
            if (eventsTabOpen)
            {
                LoadEvents();
            }
            else
            {
                LoadDeadlines();
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode != Result.Ok)
            {
                // If the activity terminated abnormally, do not attempt to add anything
                return;
            }

            if (requestCode == AddEventRequest || requestCode == EditEventRequest)
            {
                // Add the resulting event to the calendar
                Event resultEvent = new EventSerializer(data).ReadEvent(EventSerializer.ResultEvent);
                if (resultEvent != null)
                {
                    Model.Calendar.AddEvent(resultEvent);
                }
            }
            else if (requestCode == AddDeadlineRequest)
            {
                // Add the resulting deadline to the calendar
                Deadline resultDeadline = new DeadlineSerializer(data).ReadDeadline(DeadlineSerializer.ResultDeadline);
                if (resultDeadline != null)
                {
                    Model.Calendar.AddDeadline(resultDeadline);
                }
            }
        }

        /*
         * Loads every event scheduled for the current day into the main text
         * view.
         */
        private void LoadEvents()
        {
            // Load events from today
            DateTime loadedDate = DateTime.Today;

            // Create a range of DateTimes
            // We want to count midnight as belonging to the previous day.
            DateTime start = loadedDate.AddMilliseconds(1);
            DateTime end = loadedDate.AddDays(1);

            // Get the events in range from the calendar
            var loadedEvents = Model.Calendar.GetEventsInRange(start, end);

            // Sort the events
            loadedEvents.Sort();

            // Fill the main display with a list of events
            ViewUtil util = new ViewUtil(this);
            ViewUtil.ToStr<Event> label = e => e.ToString();
            ViewUtil.ToStr<Event> color = e => e.Type.colorString;
            ViewUtil.SetupCallback<Event> setup = (view, layout, e) =>
            {
                // Register an event handler to delete or edit the event
                view.LongClick += (sender, ea) =>
                {
                    var infoDialog = new AlertDialog.Builder(this);
                    infoDialog.SetMessage("Delete or edit this event?");
                    infoDialog.SetPositiveButton("Delete", delegate
                    {
                        layout.RemoveView(view);
                        Model.Calendar.RemoveEvent(e);
                    });
                    infoDialog.SetNeutralButton("Edit", delegate
                    {
                        this.StartEditEventActivity(e);
                    });
                    infoDialog.SetNegativeButton("Cancel", delegate { });
                    infoDialog.Show();
                };
            };
            util.LoadListToLayout(mainTextLayout, loadedEvents, label, color, setup);
        }

        /*
         * Loads every deadline scheduled after the current day into the main text
         * view.
         */
        private void LoadDeadlines()
        {
            // Load deadlines that have not already passed
            DateTime start = DateTime.Now;
            List<Deadline> deadlines = Model.Calendar.GetDeadlinesAfterTime(start);

            // Sort the deadlines
            deadlines.Sort();

            // Fill the main display with the list of deadlines
            ViewUtil util = new ViewUtil(this);
            ViewUtil.ToStr<Deadline> label = d => d.ToString();
            ViewUtil.ToStr<Deadline> color = d => "#F44336";
            util.LoadListToLayout(mainTextLayout, deadlines, label, color, null);
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
         * Starts the DeadlineEditorActivity. No deadline is passed to the
         * editor, so the returned event will be wholly new.
         */
        public void StartAddDeadlineActivity()
        {
            Intent intent = new Intent(this, typeof(DeadlineEditorActivity));
            StartActivityForResult(intent, AddDeadlineRequest);
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