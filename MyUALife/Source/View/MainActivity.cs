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

namespace MyUALife
{
    [Activity(Label = "MyUALife")]
    public class MainActivity : Activity
    {
        // Request codes for the EventEditorActivity
        private const int AddEventRequest = 1;
        private const int EditEventRequest = 2;
        private const int DeadlineToEventRequest = 3;

        // Request codes for the DeadlineEditorActivity
        private const int AddDeadlineRequest = 4;
        private const int EditDeadlineRequest = 5;

        //Request code for the SurveyActivity
        private const int SurveyRequest = 6;

        // The location to save/load the calendar
        private const String CalendarFileName = "calendar.bin";

        // The opened tab -- true: events tab, false: deadlines tab
        private bool eventsTabOpen = true;

        // GUI components
        private Button calendarButton;
        private Button happeningsButton;
        private Button createEventButton;
        private Button createDeadlineButton;
        private Button surveyButton;
        private LinearLayout mainTextLayout;
        private RadioButton eventsTab;
        private RadioButton deadlinesTab;

        // Helper to setup the filter spinner
        private SpinnerHelper<FilterSet> filterSpinner;

        // The currently selected filter for events and deadlines
        private FilterSet CurrentFilter
        {
            get
            {
                return filterSpinner.SelectedItem;
            }
        }

        // The Calendar that stores all the user info
        private Calendar calendar;

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
            surveyButton = FindViewById<Button>(Resource.Id.surveyButton);
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

            surveyButton.Click += (sender, e) => StartSurveyActivity();

            // Setup the create event button to open the create event screen
            createEventButton.Click += (sender, e) => StartAddEventActivity();

            // Setup the deadline button to open the create deadline screen
            createDeadlineButton.Click += (sender, e) => StartAddDeadlineActivity();

            // Setup the events tab to display the events list
            eventsTab.Click += (sender, e) => LoadEvents();

            // Setup the deadlines tab to display the deadlines list
            deadlinesTab.Click += (sender, e) => LoadDeadlines();

            // Initialize the calendar from saved file
            calendar = InitCalendar();
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Load the events scheduled for today
            UpdateCenterLayout();
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
                    calendar.AddEvent(resultEvent);
                }
            }
            else if (requestCode == DeadlineToEventRequest)
            {
                // Add the resulting event to the calendar
                Event resultEvent = new EventSerializer(data).ReadEvent(EventSerializer.ResultEvent);
                if (resultEvent != null)
                {
                    calendar.AddEvent(resultEvent);
                }
                // If no event came in, return the deadline to the calendar
                else
                {
                    Deadline resultDeadline = new DeadlineSerializer(data).ReadDeadline(DeadlineSerializer.ResultDeadline);
                    if (resultDeadline != null)
                    {
                        calendar.AddDeadline(resultDeadline);
                    }
                }
            }
            else if (requestCode == AddDeadlineRequest || requestCode == EditDeadlineRequest)
            {
                // Add the resulting deadline to the calendar
                Deadline resultDeadline = new DeadlineSerializer(data).ReadDeadline(DeadlineSerializer.ResultDeadline);
                if (resultDeadline != null)
                {
                    calendar.AddDeadline(resultDeadline);
                }
            }
            SaveCalendar();
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
            var loadedEvents = calendar.GetEventsOnDate(DateTime.Today);

            // Apply the filter
            loadedEvents = Calendar.FilterEventsByTypes(loadedEvents, CurrentFilter.AllowedTypes);

            // Sort the events
            loadedEvents.Sort();

            // Fill the main display with a list of events
            ViewUtil util = new ViewUtil(this);
            ToStr<Event> label = e => e.ToString();
            ToStr<Event> color = e => e.Type.ColorString;
            ViewUtil.SetupCallback<Event> setup = (view, layout, calEvent) =>
            {
                // Register an event handler to delete or edit the event
                view.LongClick += (sender, ea) =>
                {
                    var infoDialog = new AlertDialog.Builder(this);

                    if (calEvent.Type.IsEditable)
                    {
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

            // Indicate that the events tab is now open
            eventsTabOpen = true;
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
            ViewUtil util = new ViewUtil(this);
            ToStr<Deadline> label = d => d.ToString();
            ToStr<Deadline> color = d => "#F44336";
            ViewUtil.SetupCallback<Deadline> setup = (view, layout, deadline) =>
            {
                // Register an event handler to delete or edit the deadline
                view.LongClick += (sender, ea) =>
                {
                    var infoDialog = new AlertDialog.Builder(this);

                    if (deadline.Type.IsEditable)
                    {
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

            // Indicate that the deadliens tab is now open
            eventsTabOpen = false;
        }

        private void SendFreeTime(Intent intent)
        {
            List<Event> freeTimeEvents = calendar.GetFreeTimeOnDate(DateTime.Today);
            EventSerializer serializer = new EventSerializer(intent);
            for (int i = 0; i < freeTimeEvents.Count; i++)
            {
                String key = "FreeTime" + i;
                serializer.WriteEvent(key, freeTimeEvents[i]);
            }
            intent.PutExtra("FreeTimeCount", freeTimeEvents.Count);
        }

        private void SendPastEvents(Intent intent)
        {
            List<Event> pastEvents = calendar.GetEventsInRange(DateTime.MinValue, DateTime.Today);
            pastEvents = Calendar.FilterEventsByType(pastEvents, Category.homework);
            EventSerializer serializer = new EventSerializer(intent);
            for (int i = 0; i < pastEvents.Count; i++)
            {
                String key = "PastEvent" + i;
                serializer.WriteEvent(key, pastEvents[i]);
            }
            intent.PutExtra("PastEventCount", pastEvents.Count);
        }

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
            if (ret == null)
            {
                ret = Calendar.CreateDefaultCalendar();
            }
            return ret;
        }

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

        /*
         * Starts the EventEditorActivity. No event is passed to the editor, so
         * the returned event will be wholly new.
         */
        private void StartAddEventActivity()
        {
            Intent intent = new Intent(this, typeof(EventEditorActivity));
            SendFreeTime(intent);
            StartActivityForResult(intent, AddEventRequest);
        }

        /*
         * Starts the DeadlineEditorActivity. No deadline is passed to the
         * editor, so the returned event will be wholly new.
         */
        private void StartAddDeadlineActivity()
        {
            Intent intent = new Intent(this, typeof(DeadlineEditorActivity));
            StartActivityForResult(intent, AddDeadlineRequest);
        }

        /* 
         * Starts the SurveyActivity.
         */
        private void StartSurveyActivity()
        {
            Intent intent = new Intent(this, typeof(SurveyActivity));
            StartActivityForResult(intent, SurveyRequest);
        }

        /*
         * Starts the editor with the given event's info as the starting values
         * of the editor's components. The returned value will replace the
         * given event in the calendar.
         */
        private void StartEditEventActivity(Event calendarEvent)
        {
            Intent intent = new Intent(this, typeof(EventEditorActivity));
            SendFreeTime(intent);
            new EventSerializer(intent).WriteEvent(EventSerializer.InputEvent, calendarEvent);
            calendar.RemoveEvent(calendarEvent);
            StartActivityForResult(intent, EditEventRequest);
        }

        /*
         * Starts the editor with the given deadline's info as the starting
         * values of the editor's components. The returned value will replace
         * the given deadline in the calendar.
         */
        private void StartEditDeadlineActivity(Deadline deadline)
        {
            Intent intent = new Intent(this, typeof(DeadlineEditorActivity));
            new DeadlineSerializer(intent).WriteDeadline(DeadlineSerializer.InputDeadline, deadline);
            calendar.RemoveDeadline(deadline);
            StartActivityForResult(intent, EditDeadlineRequest);
        }

        /*
         * Starts the event editor with the given deadline's info as the
         * starting values of the editor's components. The editor is expected
         * to return either a new event, or the same deadline. If there is a
         * new event, then the deadline will be removed from the calendar.
         * Otherwise, the deadline remains in the calendar.
         */
        private void StartDeadlineToEventActivity(Deadline deadline)
        {
            Intent intent = new Intent(this, typeof(EventEditorActivity));
            SendFreeTime(intent);
            new DeadlineSerializer(intent).WriteDeadline(DeadlineSerializer.InputDeadline, deadline);
            calendar.RemoveDeadline(deadline);
            StartActivityForResult(intent, DeadlineToEventRequest);
        }
    }
}