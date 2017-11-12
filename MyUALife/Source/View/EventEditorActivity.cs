using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Support.V4.Widget;
using System;

namespace MyUALife
{
    [Activity(Label = "Create Event")]
    public class EventEditorActivity : Activity
    {
        // GUI components
        private DrawerLayout drawerLayout;
        private EditText nameText;
        private EditText descriptionText;
        private TextView startTimeLabel;
        private TextView endTimeLabel;
        private Button changeStartButton;
        private Button changeEndButton;
        private Button saveButton;
        private LinearLayout freeTimeLayout;

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
            SetContentView(Resource.Layout.EventEditor);

            // Get components from id
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerLayout);
            nameText = FindViewById<EditText>(Resource.Id.nameText);
            descriptionText = FindViewById<EditText>(Resource.Id.descriptionText);
            startTimeLabel = FindViewById<TextView>(Resource.Id.startTimeLabel);
            endTimeLabel = FindViewById<TextView>(Resource.Id.endTimeLabel);
            changeStartButton = FindViewById<Button>(Resource.Id.changeStartButton);
            changeEndButton = FindViewById<Button>(Resource.Id.changeEndButton);
            saveButton = FindViewById<Button>(Resource.Id.saveButton);
            freeTimeLayout = FindViewById<LinearLayout>(Resource.Id.freeTimeLayout);

            // Setup the DrawerLayout to keep track of its open/closed state
            drawerLayout.DrawerOpened += (sender, e) => DrawerOpen = true;
            drawerLayout.DrawerClosed += (sender, e) => DrawerOpen = false;

            // Setup the text fields to turn on the save button when edited
            nameText.TextChanged += (sender, e) => HasUnsavedChanges = true;
            descriptionText.TextChanged += (sender, e) => HasUnsavedChanges = true;

            // Configure the description text to display correctly
            descriptionText.SetHorizontallyScrolling(false);
            descriptionText.SetMaxLines(1000);

            // Setup the time buttons to display a time picker dialog
            startTime = new DateTimeFetcher(this, DateTime.Now, OnTimeChange);
            endTime = new DateTimeFetcher(this, DateTime.Now, OnTimeChange);
            changeStartButton.Click += (sender, e) => startTime.PollDateTime();
            changeEndButton.Click += (sender, e) => endTime.PollDateTime();

            // Setup the save changes button to save the current data when pressed
            saveButton.Click += (sender, e) => SaveChanges();

            // Configure the spinner to display the correct list of EventTypes
            Spinner spinner = FindViewById<Spinner>(Resource.Id.typeSpinner);
            typeSpinner = new SpinnerHelper<EventType>(spinner, Category.creatableTypes, t => t.name);

            // Setup the spinner to turn on the save button
            typeSpinner.Spinner.ItemSelected += (sender, e) => HasUnsavedChanges = true;

            // Get the event stored in Intent, if any
            Event input = new EventSerializer(Intent).ReadEvent(EventSerializer.InputEvent);
            if (input != null)
            {
                // Store data from input in the components
                nameText.Text = input.Name;
                descriptionText.Text = input.Description;
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
                    nameText.Text = deadline.Name;
                    descriptionText.Text = deadline.Description;
                    endTime.Time = deadline.Time;
                    typeSpinner.SelectedItem = deadline.associatedEventType;

                    Intent returnData = new Intent();
                    new DeadlineSerializer(returnData).WriteDeadline(DeadlineSerializer.ResultDeadline, deadline);
                    SetResult(Result.Ok, returnData);
                }
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Update the list of free time events in the pull out drawer
            LoadFreeTimeEvents();

            // Initialize the start/end time labels with the correct times
            UpdateTimeLabels();

            // Ensure that all components have the correct enabled state
            UpdateEnableStates();
        }

        /*
         * Asks the calendar for event representations of the user's free time
         * and loads the list into the pull-out LinearLayout.
         */
        private void LoadFreeTimeEvents()
        {
            // Load events from today
            DateTime loadedDate = DateTime.Today;

            // Create a range of DateTimes
            // We want to count midnight as belonging to the previous day.
            DateTime start = loadedDate.AddMilliseconds(1);
            DateTime end = loadedDate.AddDays(1);

            // Get the events in range from the calendar
            var loadedEvents = Model.Calendar.GetFreeTimeBlocksInRange(start, end);

            // Fill the free time display with a list of free times.
            ViewUtil util = new ViewUtil(this);
            ToStr<Event> label = e => String.Format("{0} - {1}", e.StartTime, e.EndTime);
            ToStr<Event> color = e => e.Type.colorString;
            ViewUtil.SetupCallback<Event> setup = (view, layout, freeTime) =>
            {
                view.Click += (sender, e) =>
                {
                    startTime.Time = freeTime.StartTime;
                    endTime.Time = freeTime.EndTime;
                    UpdateTimeLabels();
                };
            };
            util.LoadListToLayout(freeTimeLayout, loadedEvents, label, color, setup);
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
            Event resultEvent = new Event(nameText.Text, descriptionText.Text, type, startTime.Time, endTime.Time);
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
            saveButton.Enabled = HasUnsavedChanges && nameText.Text != "" && !DrawerOpen;

            nameText.Enabled = !DrawerOpen;
            descriptionText.Enabled = !DrawerOpen;
            changeStartButton.Enabled = !DrawerOpen;
            changeEndButton.Enabled = !DrawerOpen;
            typeSpinner.Spinner.Enabled = !DrawerOpen;
        }
    }
}