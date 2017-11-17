using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Support.V4.Widget;
using System;
using System.Collections.Generic;

namespace MyUALife
{
    [Activity(Label = "Survey")]
    public class SurveyActivity : Activity
    {
        /*
         * Currently this file is mostly just a copy of EventEditorActivity.cs, with a few changes to the variables
         * - Daniel
         */


        // GUI components
        private DrawerLayout drawerLayout;
        private TextView nameLabel;
        private TextView descriptionLabel;
        private TextView startTimeLabel;
        private TextView endTimeLabel;
        private TextView estimateLabel;
        private EditText timeText;
        private Button submitButton;
        private Button ignoreButton;

        // The times selected with DatePickerFragment.
        private DateTimeFetcher startTime;
        private DateTimeFetcher endTime;

        // Helper for the type spinner
        private SpinnerHelper<EventType> typeSpinner;

        // True if the user has made changes since they last saved
        private bool hasUnsavedChanges = false;
        private bool HasUnsavedChanges
        {
            get
            {
                return hasUnsavedChanges;
            }
            set
            {
                hasUnsavedChanges = value;
                UpdateEnableStates();
            }
        }

        // True when the drawer is open
        private bool drawerOpen = false;
        private bool DrawerOpen
        {
            get
            {
                return drawerOpen;
            }
            set
            {
                drawerOpen = value;
                UpdateEnableStates();
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.Survey);

            // Get components from id
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerLayout);
            nameLabel = FindViewById<EditText>(Resource.Id.nameText);
            descriptionLabel = FindViewById<EditText>(Resource.Id.descriptionText);
            startTimeLabel = FindViewById<TextView>(Resource.Id.startTimeLabel);
            endTimeLabel = FindViewById<TextView>(Resource.Id.endTimeLabel);

            // Setup the DrawerLayout to keep track of its open/closed state
            drawerLayout.DrawerOpened += (sender, e) => DrawerOpen = true;
            drawerLayout.DrawerClosed += (sender, e) => DrawerOpen = false;

            // Setup the text fields to turn on the save button when edited
            nameLabel.TextChanged += (sender, e) => HasUnsavedChanges = true;
            descriptionLabel.TextChanged += (sender, e) => HasUnsavedChanges = true;

            // Configure the description text to display correctly
            descriptionLabel.SetHorizontallyScrolling(false);
            descriptionLabel.SetMaxLines(1000);

            // Setup the time buttons to display a time picker dialog
            startTime = new DateTimeFetcher(this, DateTime.Now, OnTimeChange);
            endTime = new DateTimeFetcher(this, DateTime.Now, OnTimeChange);

            // Configure the spinner to display the correct list of EventTypes
            Spinner spinner = FindViewById<Spinner>(Resource.Id.typeSpinner);
            typeSpinner = new SpinnerHelper<EventType>(spinner, Category.creatableTypes, t => t.Name);

            // Setup the spinner to turn on the save button
            typeSpinner.Spinner.ItemSelected += (sender, e) => HasUnsavedChanges = true;

            // Get the event stored in Intent, if any
            Event input = new EventSerializer(Intent).ReadEvent(EventSerializer.InputEvent);
            if (input != null)
            {
                // Store data from input in the components
                nameLabel.Text = input.Name;
                descriptionLabel.Text = input.Description;
                startTime.Time = input.StartTime;
                endTime.Time = input.EndTime;
                typeSpinner.SelectedItem = input.Type;
                SaveChanges();
            }
            else
            {
                Deadline deadline = new DeadlineSerializer(Intent).ReadDeadline(DeadlineSerializer.InputDeadline);
                if (deadline != null)
                {
                    nameLabel.Text = deadline.Name;
                    descriptionLabel.Text = deadline.Description;
                    endTime.Time = deadline.Time;
                    typeSpinner.SelectedItem = deadline.Type;

                    Intent returnData = new Intent();
                    new DeadlineSerializer(returnData).WriteDeadline(DeadlineSerializer.ResultDeadline, deadline);
                    SetResult(Result.Ok, returnData);
                }
            }

            // Load the free time blocks from the intent
            EventSerializer deserializer = new EventSerializer(Intent);
            List<Event> freeTimeEvents = new List<Event>();
            int count = Intent.GetIntExtra("FreeTimeCount", 0);
            for (int i = 0; i < count; i++)
            {
                Event freeTime = deserializer.ReadEvent("FreeTime" + i);
                if (freeTime != null)
                {
                    freeTimeEvents.Add(freeTime);
                }
            }

            // Fill the free time display with a list of free times.
            ViewUtil util = new ViewUtil(this);
            ToStr<Event> label = e => String.Format("{0} - {1}", e.StartTime, e.EndTime);
            ToStr<Event> color = e => e.Type.ColorString;
            ViewUtil.SetupCallback<Event> setup = (view, layout, freeTime) =>
            {
                view.Click += (sender, e) =>
                {
                    startTime.Time = freeTime.StartTime;
                    endTime.Time = freeTime.EndTime;
                    UpdateTimeLabels();
                };
            };
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Initialize the start/end time labels with the correct times
            UpdateTimeLabels();

            // Ensure that all components have the correct enabled state
            UpdateEnableStates();
        }

        /*
         * Transfers the data entered into the GUI elements into the fields of
         * the current Event. If there is no current Event, this method creates
         * one and stores it in the Calendar. Any additional data that needs to
         * be returned to the caller can be stored in the input intent.
         */
        private void SaveChanges(Intent data)
        {
            EventType type = typeSpinner.SelectedItem;
            Event resultEvent = new Event(nameLabel.Text, descriptionLabel.Text, type, startTime.Time, endTime.Time);
            new EventSerializer(data).WriteEvent(EventSerializer.ResultEvent, resultEvent);
            SetResult(Result.Ok, data);
            HasUnsavedChanges = false;
        }

        /*
         * Saves changes with no additional data.
         */
        private void SaveChanges()
        {
            SaveChanges(new Intent());
        }

        /*
         * Updates the time labels that display the selected start and end
         * times to reflect the DateTimes selected by the user.
         */
        private void UpdateTimeLabels()
        {
            startTimeLabel.Text = startTime.Time.ToString("g");
            endTimeLabel.Text = endTime.Time.ToString("g");
        }

        /*
         * This method should be called whenever the user uses the time changer
         * buttons to set the start or end time for an event.
         */
        private void OnTimeChange()
        {
            if (endTime.Time < startTime.Time.AddMinutes(1))
            {
                endTime.Time = startTime.Time.AddMinutes(1);
            }
            UpdateTimeLabels();
            HasUnsavedChanges = true;
        }

        /*
         * Determines whether each component of this view should be enabled or
         * disabled and updates them appropriately.
         */
        private void UpdateEnableStates()
        {
            nameLabel.Enabled = !DrawerOpen;
            descriptionLabel.Enabled = !DrawerOpen;
            typeSpinner.Spinner.Enabled = !DrawerOpen;
        }
    }
}