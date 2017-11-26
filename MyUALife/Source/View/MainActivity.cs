using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyUALife
{
    [Activity(Label = "MyUALife", MainLauncher = true)]
    public class MainActivity : Activity
    {
        // Request codes for the event and deadline editors
        private const int AddEventRequest = 1;
        private const int EditEventRequest = 2;
        private const int DeadlineToEventRequest = 3;
        private const int AddDeadlineRequest = 4;
        private const int EditDeadlineRequest = 5;

        // Keys for passing information through Intent extras
        public const String InputDeadline = "MyUALife.InputDeadline";
        public const String ResultDeadline = "MyUALife.ResultDeadline";
        public const String InputEvent = "MyUALife.InputEvent";
        public const String ResultEvent = "MyUALife.ResultEvent";
        public const String RecurrenceKey = "MyUALife.Recurrence";

        // The location to save/load the calendar
        private const String CalendarFileName = "calendar.bin";

        // GUI components
        private Button createEventButton;
        private Button createDeadlineButton;
        private LinearLayout eventsLayout;
        private RadioButton eventsTab;
        private RadioButton deadlinesTab;
        private ImageButton backButton;
        private ImageButton forwardButton;
        private TextView dateLabel;

        // Helper to setup the filter spinner
        private SpinnerHelper<FilterSet> filterSpinner;

        // The Calendar that stores all the user info
        private Calendar calendar;

        // The date whose events the user is currently viewing
        private DateTime loadedDate = DateTime.Now;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.Main);

            // Get components by id
            createEventButton = FindViewById<Button>(Resource.Id.createEventButton);
            createDeadlineButton = FindViewById<Button>(Resource.Id.createDeadlineButton);
            eventsLayout = FindViewById<LinearLayout>(Resource.Id.eventsLayout);
            eventsTab = FindViewById<RadioButton>(Resource.Id.eventsRadioButton);
            deadlinesTab = FindViewById<RadioButton>(Resource.Id.deadlinesRadioButton);
            backButton = FindViewById<ImageButton>(Resource.Id.backButton);
            forwardButton = FindViewById<ImageButton>(Resource.Id.forwardButton);
            dateLabel = FindViewById<TextView>(Resource.Id.dateLabel);

            // Setup the filter button to filter events
            Spinner spinner = FindViewById<Spinner>(Resource.Id.filterSpinner);
            filterSpinner = new SpinnerHelper<FilterSet>(spinner, FilterSet.FilterSets, f => f.Name);
            filterSpinner.Spinner.ItemSelected += (sender, e) => UpdateEventsLayout();

            // Setup the create event button to open the create event screen
            createEventButton.Click += (sender, e) => StartAddEventActivity();

            // Setup the deadline button to open the create deadline screen
            createDeadlineButton.Click += (sender, e) => StartAddDeadlineActivity();

            // Setup the events/deadlines tabs to update the events layout
            eventsTab.Click += (sender, e) => UpdateEventsLayout();
            deadlinesTab.Click += (sender, e) => UpdateEventsLayout();

            // Setup the date changer buttons to change the date
            backButton.Click += (sender, e) => ShiftDate(-1);
            forwardButton.Click += (sender, e) => ShiftDate(1);

            // Initialize the calendar from saved file
            calendar = InitCalendar();
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Load the events scheduled for today
            UpdateEventsLayout();

            // Update the date label
            UpdateDateLabel();
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
                LoadResultEvent(data);
            }
            else if (requestCode == DeadlineToEventRequest)
            {
                // If the event is not found, return the deadline to the calendar
                if (!LoadResultEvent(data))
                {
                    Deadline resultDeadline = Deadline.ReadDeadline(data, ResultDeadline);
                    if (resultDeadline != null)
                    {
                        calendar.AddDeadline(resultDeadline);
                    }
                }
            }
            else if (requestCode == AddDeadlineRequest || requestCode == EditDeadlineRequest)
            {
                // Add the resulting deadline to the calendar
                Deadline resultDeadline = Deadline.ReadDeadline(data, ResultDeadline);
                if (resultDeadline != null)
                {
                    calendar.AddDeadline(resultDeadline);
                }
            }
            SaveCalendar();
        }

        /*
         * Reads an event from the given intent, if any, and saves it to the
         * calendar. If the intent specifies that the event should recurr,
         * a RecurringEventGenerator will be added instead.
         */
        private bool LoadResultEvent(Intent intent)
        {
            // Add the resulting event to the calendar, if possible
            Event resultEvent = Event.ReadEvent(intent, ResultEvent);
            if (resultEvent == null)
            {
                return false;
            }

            bool[] recurrences = intent.GetBooleanArrayExtra(RecurrenceKey);
            if (!Array.Exists(recurrences, b => b))
            {
                calendar.AddEvent(resultEvent);
                return true;
            }

            DateTime now = DateTime.Now;
            int currentDay = (int) now.DayOfWeek;
            for (int i = 0; i < 7; i++)
            {
                if (!recurrences[i])
                {
                    continue;
                }
                int dayOffset = (i - currentDay + 7) % 7;
                DateTime date = now.Date.AddDays(dayOffset);
                DateTime start = date + resultEvent.StartTime.TimeOfDay;
                DateTime end = date + resultEvent.EndTime.TimeOfDay;
                Event baseEvent = new Event(resultEvent.Name, resultEvent.Description, resultEvent.Type, start, end);
                var gen = new RecurringEventGenerator(baseEvent, new TimeSpan(7, 0, 0, 0));
                calendar.AddEvent(baseEvent);
                calendar.AddRecurringEvent(gen);
            }
            return true;
        }

        /*
         * Loads the current events or deadlines into the mainTextLayout
         */
        private void UpdateEventsLayout()
        {
            if (eventsTab.Checked)
            {
                LoadEvents();
            }
            else
            {
                LoadDeadlines();
            }
        }

        /*
         * Updates the label that displays the current date.
         */
        private void UpdateDateLabel()
        {
            dateLabel.Text = loadedDate.ToString("D");
        }

        /*
         * Adds i days to the current selected date.
         */
        private void ShiftDate(int i)
        {
            loadedDate = loadedDate.AddDays(i);
            UpdateDateLabel();
            if (eventsTab.Checked)
            {
                LoadEvents();
            }
        }

        /*
         * Loads every event scheduled for the current day into the main text
         * view.
         */
        private void LoadEvents()
        {
            // Get the events in range from the calendar
            var loadedEvents = calendar.GetEventsOnDate(loadedDate);

            // Apply the filter
            var types = filterSpinner.SelectedItem.AllowedTypes;
            loadedEvents = Calendar.FilterEventsByTypes(loadedEvents, types);

            // Sort the events
            loadedEvents.Sort();

            // Fill the main display with a list of events
            ToStr<Event> label = e => e.ToString();
            ToStr<Event> color = e => e.Type.ColorString;
            ViewUtil.SetupCallback<Event> setup = (view, layout, calEvent) =>
            {
                // Register an event handler to delete or edit the event
                view.LongClick += (sender, ea) =>
                {
                    var infoDialog = new AlertDialog.Builder(this);

                    infoDialog.SetMessage("Delete or edit this event?");
                    infoDialog.SetPositiveButton("Delete", delegate
                    {
                        layout.RemoveView(view);
                        calendar.RemoveEvent(calEvent);
                        SaveCalendar();
                    });
                    infoDialog.SetNeutralButton("Edit", delegate
                    {
                        StartEditEventActivity(calEvent);
                    });
                    infoDialog.SetNegativeButton("Cancel", delegate { });

                    infoDialog.Show();
                };
            };
            ViewUtil.LoadListToLayout(eventsLayout, loadedEvents, label, color, setup);
        }

        /*
         * Loads every deadline scheduled after the current day into the main text
         * view.
         */
        private void LoadDeadlines()
        {
            // Load deadlines that have not already passed
            DateTime start = DateTime.Now;
            List<Deadline> deadlines = calendar.GetDeadlinesAfterTime(start);

            // Sort the deadlines
            deadlines.Sort();

            // Fill the main display with the list of deadlines
            ToStr<Deadline> label = d => d.ToString();
            ToStr<Deadline> color = d => "#F44336";
            ViewUtil.SetupCallback<Deadline> setup = (view, layout, deadline) =>
            {
                // Register an event handler to delete or edit the deadline
                view.LongClick += (sender, ea) =>
                {
                    var infoDialog = new AlertDialog.Builder(this);

                    infoDialog.SetMessage("Delete or edit this deadline?");
                    infoDialog.SetPositiveButton("Delete", delegate
                    {
                        layout.RemoveView(view);
                        calendar.RemoveDeadline(deadline);
                        SaveCalendar();
                    });
                    infoDialog.SetNeutralButton("Edit", delegate
                    {
                        StartEditDeadlineActivity(deadline);
                    });
                    infoDialog.SetNegativeButton("Cancel", delegate { });

                    infoDialog.Show();
                };

                // Register an event handler to create an event from the deadline
                view.Click += (sender, ea) =>
                {
                    var infoDialog = new AlertDialog.Builder(this);
                    infoDialog.SetMessage("Create an event from this deadline?");
                    infoDialog.SetPositiveButton("Ok", delegate
                    {
                        StartDeadlineToEventActivity(deadline);
                    });
                    infoDialog.SetNegativeButton("Cancel", delegate { });
                    infoDialog.Show();
                };
            };
            ViewUtil.LoadListToLayout(eventsLayout, deadlines, label, color, setup);
        }

        /*
         * Sends a list of free time events to the given intent.
         */
        private void SendFreeTime(Intent intent)
        {
            List<Event> freeTimeEvents = calendar.GetFreeTimeOnDate(DateTime.Today);
            for (int i = 0; i < freeTimeEvents.Count; i++)
            {
                Event.WriteEvent(intent, "FreeTime" + i, freeTimeEvents[i]);
            }
            intent.PutExtra("FreeTimeCount", freeTimeEvents.Count);
        }

        /*
         * Reads the calendar save file. If the file could be read, then the
         * result is returned. Otherwise, the default calendar is returned.
         */
        private Calendar InitCalendar()
        {
            Stream input = null;
            Calendar ret = null;
            try
            {
                input = OpenFileInput(CalendarFileName);
                BinaryFormatter formatter = new BinaryFormatter();
                ret = (Calendar) formatter.Deserialize(input);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            finally
            {
                input?.Close();
            }
            return ret ?? Calendar.CreateDefaultCalendar();
        }

        /*
         * Saves the state of the calendar to a file.
         */
        private void SaveCalendar()
        {
            Stream output = null;
            try
            {
                output = OpenFileOutput(CalendarFileName, FileCreationMode.Private);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(output, calendar);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            finally
            {
                output?.Close();
            }
        }
        
        /* Start Activity Methods */
        
        /*
         * Starts the event editor with no input.
         */
        private void StartAddEventActivity()
        {
            Intent intent = new Intent(this, typeof(EventEditorActivity));
            SendFreeTime(intent);
            StartActivityForResult(intent, AddEventRequest);
        }

        /*
         * Starts the deadline editor with no input.
         */
        private void StartAddDeadlineActivity()
        {
            Intent intent = new Intent(this, typeof(DeadlineEditorActivity));
            StartActivityForResult(intent, AddDeadlineRequest);
        }

        /*
         * Starts the event editor with the given event's properties as the
         * default values. The given event will be replaced by the result.
         */
        private void StartEditEventActivity(Event calendarEvent)
        {
            Intent intent = new Intent(this, typeof(EventEditorActivity));
            SendFreeTime(intent);
            Event.WriteEvent(intent, InputEvent, calendarEvent);
            calendar.RemoveEvent(calendarEvent);
            StartActivityForResult(intent, EditEventRequest);
        }

        /*
         * Starts the deadline editor with the given deadline's properties as
         * the default values. The given deadline will be replaced by the
         * result.
         */
        private void StartEditDeadlineActivity(Deadline deadline)
        {
            Intent intent = new Intent(this, typeof(DeadlineEditorActivity));
            Deadline.WriteDeadline(intent, InputDeadline, deadline);
            calendar.RemoveDeadline(deadline);
            StartActivityForResult(intent, EditDeadlineRequest);
        }

        /*
         * Starts the event editor with the given deadline's properties as the
         * default values. The given deadline will be replaced by the result.
         */
        private void StartDeadlineToEventActivity(Deadline deadline)
        {
            Intent intent = new Intent(this, typeof(EventEditorActivity));
            SendFreeTime(intent);
            Deadline.WriteDeadline(intent, InputDeadline, deadline);
            calendar.RemoveDeadline(deadline);
            StartActivityForResult(intent, DeadlineToEventRequest);
        }
    }
}