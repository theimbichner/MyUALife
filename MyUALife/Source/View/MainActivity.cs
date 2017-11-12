using Android.App;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Android.Content;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Java.IO;

namespace MyUALife
{
    [Activity(Label = "MyUALife", MainLauncher = true)]
    public class MainActivity : Activity
    {
        // Request codes for the EventEditorActivity
        private const int AddEventRequest = 1;
        private const int EditEventRequest = 2;
        private const int DeadlineToEventRequest = 3;

        // Request codes for the DeadlineEditorActivity
        private const int AddDeadlineRequest = 4;
        private const int EditDeadlineRequest = 5;

        // The filename for the saved calendar
        private const String fileName = "calendar_save_state.bin";

        // The opened tab -- true: events tab, false: deadlines tab
        private bool eventsTabOpen = true;

        // GUI components
        private Button calendarButton;
        private Button happeningsButton;
        private Button createEventButton;
        private Button createDeadlineButton;
        private LinearLayout mainTextLayout;
        private RadioButton eventsTab;
        private RadioButton deadlinesTab;

        // Helper to setup the filter spinner
        SpinnerHelper<FilterSet> filterSpinner;

        // The currently selected filter for events and deadlines
        private FilterSet Filter
        {
            get
            {
                return filterSpinner.SelectedItem;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.Main);

            // Get components by id
            calendarButton = FindViewById<Button>(Resource.Id.calendarButton);
            happeningsButton = FindViewById<Button>(Resource.Id.happeningsButton);
            createEventButton = FindViewById<Button>(Resource.Id.createEventButton);
            createDeadlineButton = FindViewById<Button>(Resource.Id.createDeadlineButton);
            mainTextLayout = FindViewById<LinearLayout>(Resource.Id.mainTextLayout);
            eventsTab = FindViewById<RadioButton>(Resource.Id.eventsRadioButton);
            deadlinesTab = FindViewById<RadioButton>(Resource.Id.deadlinesRadioButton);

            // Setup the filter button to filter events
            Spinner spinner = FindViewById<Spinner>(Resource.Id.filterSpinner);
            filterSpinner = new SpinnerHelper<FilterSet>(spinner, FilterSet.FilterSets, f => f.Name);
            filterSpinner.Spinner.ItemSelected += (sender, e) => UpdateCenterLayout();

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
            UpdateCenterLayout();
        }

        protected override void OnStop()
        {
            base.OnStop();
            Stream fos = OpenFileOutput(fileName, FileCreationMode.Private);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(fos, Model.Calendar);
            fos.Close();
            /*
            try
            {
                Stream fileStream = File.Create("calendar_save_state.bin");
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(fileStream, Model.Calendar);
                fileStream.Close();
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }
            */
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
            else if (requestCode == DeadlineToEventRequest)
            {
                // Add the resulting event to the calendar
                Event resultEvent = new EventSerializer(data).ReadEvent(EventSerializer.ResultEvent);
                if (resultEvent != null)
                {
                    Model.Calendar.AddEvent(resultEvent);
                }
                // If no event came in, return the deadline to the calendar
                else
                {
                    Deadline resultDeadline = new DeadlineSerializer(data).ReadDeadline(DeadlineSerializer.ResultDeadline);
                    if (resultDeadline != null)
                    {
                        Model.Calendar.AddDeadline(resultDeadline);
                    }
                }
            }
            else if (requestCode == AddDeadlineRequest || requestCode == EditDeadlineRequest)
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
         * Loads the current events or deadlines into the mainTextLayout
         */
        private void UpdateCenterLayout()
        {
            if (eventsTabOpen)
            {
                LoadEvents();
            }
            else
            {
                LoadDeadlines();
            }
        }

        /*
         * Loads every event scheduled for the current day into the main text
         * view.
         */
        private void LoadEvents()
        {
            // Get the events in range from the calendar
            var loadedEvents = Model.Calendar.GetEventsOnDate(DateTime.Today);

            // Apply the filter
            loadedEvents = Calendar.FilterEventsByTypes(loadedEvents, Filter.AllowedTypes);

            // Sort the events
            loadedEvents.Sort();

            // Fill the main display with a list of events
            ViewUtil util = new ViewUtil(this);
            ToStr<Event> label = e => e.ToString();
            ToStr<Event> color = e => e.Type.colorString;
            ViewUtil.SetupCallback<Event> setup = (view, layout, calEvent) =>
            {
                // Register an event handler to delete or edit the event
                view.LongClick += (sender, ea) =>
                {
                    var infoDialog = new AlertDialog.Builder(this);

                    if (calEvent.Type.editable)
                    {
                        infoDialog.SetMessage("Delete or edit this event?");
                        infoDialog.SetPositiveButton("Delete", delegate
                        {
                            layout.RemoveView(view);
                            Model.Calendar.RemoveEvent(calEvent);
                        });
                        infoDialog.SetNeutralButton("Edit", delegate
                        {
                            StartEditEventActivity(calEvent);
                        });
                        infoDialog.SetNegativeButton("Cancel", delegate { });
                    }
                    else
                    {
                        infoDialog.SetMessage("This event cannot be edited.");
                        infoDialog.SetPositiveButton("Ok", delegate { });
                    }

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
            ToStr<Deadline> label = d => d.ToString();
            ToStr<Deadline> color = d => "#F44336";
            ViewUtil.SetupCallback<Deadline> setup = (view, layout, deadline) =>
            {
                // Register an event handler to delete or edit the deadline
                view.LongClick += (sender, ea) =>
                {
                    var infoDialog = new AlertDialog.Builder(this);

                    if (deadline.associatedEventType.editable)
                    {
                        infoDialog.SetMessage("Delete or edit this deadline?");
                        infoDialog.SetPositiveButton("Delete", delegate
                        {
                            layout.RemoveView(view);
                            Model.Calendar.RemoveDeadline(deadline);
                        });
                        infoDialog.SetNeutralButton("Edit", delegate
                        {
                            StartEditDeadlineActivity(deadline);
                        });
                        infoDialog.SetNegativeButton("Cancel", delegate { });
                    }
                    else
                    {
                        infoDialog.SetMessage("This deadline cannot be edited.");
                        infoDialog.SetPositiveButton("Ok", delegate { });
                    }

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
            util.LoadListToLayout(mainTextLayout, deadlines, label, color, setup);
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
         * Starts the editor with the given event's info as the starting values
         * of the editor's components. The returned value will replace the
         * given event in the calendar.
         */
        public void StartEditEventActivity(Event calendarEvent)
        {
            Intent intent = new Intent(this, typeof(EventEditorActivity));
            new EventSerializer(intent).WriteEvent(EventSerializer.InputEvent, calendarEvent);
            Model.Calendar.RemoveEvent(calendarEvent);
            StartActivityForResult(intent, EditEventRequest);
        }

        /*
         * Starts the editor with the given deadline's info as the starting
         * values of the editor's components. The returned value will replace
         * the given deadline in the calendar.
         */
        public void StartEditDeadlineActivity(Deadline deadline)
        {
            Intent intent = new Intent(this, typeof(DeadlineEditorActivity));
            new DeadlineSerializer(intent).WriteDeadline(DeadlineSerializer.InputDeadline, deadline);
            Model.Calendar.RemoveDeadline(deadline);
            StartActivityForResult(intent, EditDeadlineRequest);
        }

        /*
         * Starts the event editor with the given deadline's info as the
         * starting values of the editor's components. The editor is expected
         * to return either a new event, or the same deadline. If there is a
         * new event, then the deadline will be removed from the calendar.
         * Otherwise, the deadline remains in the calendar.
         */
        public void StartDeadlineToEventActivity(Deadline deadline)
        {
            Intent intent = new Intent(this, typeof(EventEditorActivity));
            new DeadlineSerializer(intent).WriteDeadline(DeadlineSerializer.InputDeadline, deadline);
            Model.Calendar.RemoveDeadline(deadline);
            StartActivityForResult(intent, DeadlineToEventRequest);
        }
    }
}