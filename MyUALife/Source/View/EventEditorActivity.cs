using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Support.V4.Widget;
using System;
using System.Collections.Generic;

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
        private Spinner typeSpinner;
        private LinearLayout freeTimeLayout;

        // The times selected with DatePickerFragment.
        private DateTimeFetcher StartTime;
        private DateTimeFetcher EndTime;

        // True iff the save button should be enabled
        private bool saveButtonEnabled;

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
            typeSpinner = FindViewById<Spinner>(Resource.Id.typeSpinner);
            freeTimeLayout = FindViewById<LinearLayout>(Resource.Id.freeTimeLayout);

            drawerLayout.DrawerOpened += (sender, e) =>
            {
                nameText.Enabled = false;
                descriptionText.Enabled = false;
                changeStartButton.Enabled = false;
                changeEndButton.Enabled = false;
                saveButton.Enabled = false;
                typeSpinner.Enabled = false;
            };

            drawerLayout.DrawerClosed += (sender, e) =>
            {
                nameText.Enabled = true;
                descriptionText.Enabled = true;
                changeStartButton.Enabled = true;
                changeEndButton.Enabled = true;
                saveButton.Enabled = saveButtonEnabled;
                typeSpinner.Enabled = true;
            };

            // Setup the text fields to turn on the save button when edited
            nameText.TextChanged += (sender, e) => TurnOnSaveButton();
            descriptionText.TextChanged += (sender, e) => TurnOnSaveButton();

            // Configure the description text to display correctly
            descriptionText.SetHorizontallyScrolling(false);
            descriptionText.SetMaxLines(1000);

            // Setup the time buttons to display a time picker dialog
            StartTime = new DateTimeFetcher(this, DateTime.Now, OnTimeChange);
            EndTime = new DateTimeFetcher(this, DateTime.Now, OnTimeChange);
            changeStartButton.Click += (sender, e) => StartTime.PollDateTime();
            changeEndButton.Click += (sender, e) => EndTime.PollDateTime();

            // Setup the save changes button to save the current data when pressed
            saveButton.Click += (sender, e) => SaveChanges();

            // Disable the save changes button by default
            saveButtonEnabled = false;
            saveButton.Enabled = false;

            // Configure the spinner to display the correct list of EventTypes
            List<String> names = new List<String>();
            foreach (EventType t in Category.creatableTypes)
            {
                names.Add(t.name);
            }
            ArrayAdapter<String> adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, names);
            typeSpinner.Adapter = adapter;

            // Setup the spinner to turn on the save button
            typeSpinner.ItemSelected += (sender, e) => TurnOnSaveButton();

            // Get the event stored in Intent, if any
            Event input = new EventSerializer(Intent).ReadEvent(EventSerializer.InputEvent);
            if (input != null)
            {
                // Store data from input in the components
                nameText.Text = input.Name;
                descriptionText.Text = input.Description;
                StartTime.Time = input.StartTime;
                EndTime.Time = input.EndTime;
                typeSpinner.SetSelection(adapter.GetPosition(input.Type.name));
                SaveChanges();
            }

            // Initialize the start/end time labels with the correct times
            UpdateTimeLabels();
        }

        protected override void OnStart()
        {
            base.OnStart();
            LoadFreeTimeEvents();
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
            // Hence, we start our range just after midnight
            DateTime start = loadedDate.AddMilliseconds(1);
            DateTime end = loadedDate.AddDays(1);

            // Get the events in range from the calendar
            var loadedEvents = Model.Calendar.GetFreeTimeBlocksInRange(start, end);

            // Clear the layout and add a text view for every event
            ViewUtil util = new ViewUtil(this);
            util.LoadFreeTimeToLayout(freeTimeLayout, loadedEvents);
        }

        /*
         * Transfers the data entered into the GUI elements into the fields of
         * the current Event. If there is no current Event, this method creates
         * one and stores it in the Calendar.
         */
        private void SaveChanges()
        {
            String typeName = typeSpinner.SelectedItem.ToString();
            Event resultEvent = new Event(nameText.Text, descriptionText.Text, Category.GetTypeByName(typeName), StartTime.Time, EndTime.Time);
            Intent data = new Intent();
            new EventSerializer(data).WriteEvent(EventSerializer.ResultEvent, resultEvent);
            SetResult(Result.Ok, data);
            saveButtonEnabled = false;
            saveButton.Enabled = false;
        }

        /*
         * Updates the time labels that display the selected start and end
         * times to reflect the DateTimes selected by the user.
         */
        private void UpdateTimeLabels()
        {
            startTimeLabel.Text = StartTime.Time.ToString("g");
            endTimeLabel.Text = EndTime.Time.ToString("g");
        }

        /*
         * Enables the save changes button. Anything that causes a change in
         * the data stored in the GUI components should call this method.
         */
        private void TurnOnSaveButton()
        {
            if (nameText.Text == "")
            {
                return;
            }
            saveButtonEnabled = true;
            saveButton.Enabled = true;
        }

        /*
         * This method should be called whenever the user uses the time changer
         * buttons to set the start or end time for an event.
         */
        private void OnTimeChange()
        {
            if (EndTime.Time < StartTime.Time)
            {
                EndTime.Time = StartTime.Time;
            }
            UpdateTimeLabels();
            TurnOnSaveButton();
        }
    }
}