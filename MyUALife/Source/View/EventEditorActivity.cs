using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Support.V4.Widget;
using Android.Views;
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
        private CheckBox recurringCheckBox;
        private CheckBox[] weekdayCheckBoxes = new CheckBox[7];
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.EventEditor);

            // Get components from id
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawerLayout);
            nameText = FindViewById<EditText>(Resource.Id.nameText);
            descriptionText = FindViewById<EditText>(Resource.Id.descriptionText);
            recurringCheckBox = FindViewById<CheckBox>(Resource.Id.recurringCheckBox);
            weekdayCheckBoxes[0] = FindViewById<CheckBox>(Resource.Id.sundayCheckBox);
            weekdayCheckBoxes[1] = FindViewById<CheckBox>(Resource.Id.mondayCheckBox);
            weekdayCheckBoxes[2] = FindViewById<CheckBox>(Resource.Id.tuesdayCheckBox);
            weekdayCheckBoxes[3] = FindViewById<CheckBox>(Resource.Id.wednesdayCheckBox);
            weekdayCheckBoxes[4] = FindViewById<CheckBox>(Resource.Id.thursdayCheckBox);
            weekdayCheckBoxes[5] = FindViewById<CheckBox>(Resource.Id.fridayCheckBox);
            weekdayCheckBoxes[6] = FindViewById<CheckBox>(Resource.Id.saturdayCheckBox);
            startTimeLabel = FindViewById<TextView>(Resource.Id.startTimeLabel);
            endTimeLabel = FindViewById<TextView>(Resource.Id.endTimeLabel);
            changeStartButton = FindViewById<Button>(Resource.Id.changeStartButton);
            changeEndButton = FindViewById<Button>(Resource.Id.changeEndButton);
            saveButton = FindViewById<Button>(Resource.Id.saveButton);
            freeTimeLayout = FindViewById<LinearLayout>(Resource.Id.freeTimeLayout);

            // Setup the text fields to turn on the save button when edited
            nameText.TextChanged += (sender, e) => HasUnsavedChanges = true;
            descriptionText.TextChanged += (sender, e) => HasUnsavedChanges = true;

            // Configure the description text to display correctly
            descriptionText.SetHorizontallyScrolling(false);
            descriptionText.SetMaxLines(1000);

            // Setup the recurring checkbox to show hide the other checkboxes
            recurringCheckBox.Click += (sender, e) =>
            {
                UpdateRecurrenceVisisbility();
                UpdateTimeLabels();
            };

            // Setup the time buttons to display a time picker dialog
            DateTime time = DateTime.Now;
            startTime = new DateTimeFetcher(this, time, OnTimeChange);
            endTime = new DateTimeFetcher(this, time.AddMinutes(1), OnTimeChange);
            changeStartButton.Click += (sender, e) => startTime.PollDateTime(!recurringCheckBox.Checked);
            changeEndButton.Click += (sender, e) => endTime.PollDateTime(!recurringCheckBox.Checked);

            // Setup the save changes button to save the current data when pressed
            saveButton.Click += (sender, e) => SaveChanges();

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
                    drawerLayout.CloseDrawers();
                };
            };
            util.LoadListToLayout(freeTimeLayout, freeTimeEvents, label, color, setup);
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Initialize the start/end time labels with the correct times
            UpdateTimeLabels();

            // Update the visibility of recurrence componentes
            UpdateRecurrenceVisisbility();

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
            startTimeLabel.Text = "";
            endTimeLabel.Text = "";
            if (!recurringCheckBox.Checked)
            {
                startTimeLabel.Text += startTime.Time.ToString("D") + "\n";
                endTimeLabel.Text += endTime.Time.ToString("D") + "\n";
            }
            startTimeLabel.Text += startTime.Time.ToString("t");
            endTimeLabel.Text += endTime.Time.ToString("t");
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
            saveButton.Enabled = HasUnsavedChanges && nameText.Text != "";
        }

        /*
         * Determines whether the components relating to event recurrence
         * should be visible or invisible and updates them appropriately.
         */
        private void UpdateRecurrenceVisisbility()
        {
            ViewStates viewState = ViewStates.Gone;
            if (recurringCheckBox.Checked)
            {
                viewState = ViewStates.Visible;
            }

            foreach (CheckBox c in weekdayCheckBoxes)
            {
                c.Visibility = viewState;
                c.Checked &= recurringCheckBox.Checked;
            }
        }
    }
}