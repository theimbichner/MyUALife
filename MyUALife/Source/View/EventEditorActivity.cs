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
        private LinearLayout contentLayout;
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
            contentLayout = FindViewById<LinearLayout>(Resource.Id.contentLayout);
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

            // Hook into the content root layout to detect the ime
            contentLayout.ViewTreeObserver.GlobalLayout += (sender, e) => CheckIME();

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
                HasUnsavedChanges = true;
            };

            // Setup the day of week checkboxes to turn on the save button
            foreach (CheckBox c in weekdayCheckBoxes)
            {
                c.Click += (sender, e) => HasUnsavedChanges = true;
            }

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
            typeSpinner = new SpinnerHelper<EventType>(spinner, Category.CreatableTypes, t => t.Name);

            // Setup the spinner to turn on the save button
            typeSpinner.Spinner.ItemSelected += (sender, e) => HasUnsavedChanges = true;

            // Parse any input
            LoadInputs();
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

        private void LoadInputs()
        {
            // Get the input stored in Intent, if any
            Event inputEvent = Event.ReadEvent(Intent, MainActivity.InputEvent);
            Deadline inputDeadline = Deadline.ReadDeadline(Intent, MainActivity.InputDeadline);

            if (inputEvent != null)
            {
                // Store data from input in the components
                nameText.Text = inputEvent.Name;
                descriptionText.Text = inputEvent.Description;
                startTime.Time = inputEvent.StartTime;
                endTime.Time = inputEvent.EndTime;
                typeSpinner.SelectedItem = inputEvent.Type;

                // Return the event so it remains in the calendar if the user cancels
                SaveChanges();
                Title = "Edit Event";
            }
            else if (inputDeadline != null)
            {
                // Store data from the deadline in the components
                nameText.Text = inputDeadline.Name;
                descriptionText.Text = inputDeadline.Description;
                endTime.Time = inputDeadline.Time;
                typeSpinner.SelectedItem = inputDeadline.Type;

                // Return the deadline so it remains in the calendar if the user cancels
                Intent returnData = new Intent();
                Deadline.WriteDeadline(returnData, MainActivity.ResultDeadline, inputDeadline);
                SetResult(Result.Ok, returnData);
            }

            // Load the free time blocks from the intent
            List<Event> freeTimeEvents = new List<Event>();
            int count = Intent.GetIntExtra("FreeTimeCount", 0);
            for (int i = 0; i < count; i++)
            {
                Event freeTime = Event.ReadEvent(Intent, "FreeTime" + i);
                if (freeTime != null)
                {
                    freeTimeEvents.Add(freeTime);
                }
            }

            // Fill the free time display with a list of free times.
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
            ViewUtil.LoadListToLayout(freeTimeLayout, freeTimeEvents, label, color, setup);
        }

        /*
         * Transfers the data entered into the GUI elements into an intent
         * extra and returns that extra to the main activity.
         */
        private void SaveChanges()
        {
            Intent data = new Intent();
            EventType type = typeSpinner.SelectedItem;
            Event resultEvent = new Event(nameText.Text, descriptionText.Text, type, startTime.Time, endTime.Time);
            Event.WriteEvent(data, MainActivity.ResultEvent, resultEvent);
            bool[] recurrences = new bool[7];
            for (int i = 0; i < 7; i++)
            {
                recurrences[i] = weekdayCheckBoxes[i].Checked;
            }
            data.PutExtra(MainActivity.RecurrenceKey, recurrences);
            SetResult(Result.Ok, data);
            HasUnsavedChanges = false;
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
         * Determines whether the save button should be enabled or disabled and
         * updates them appropriately.
         */
        private void UpdateEnableStates()
        {
            bool recurrenceOk = true;
            if (recurringCheckBox.Checked)
            {
                recurrenceOk = false;
                foreach (CheckBox c in weekdayCheckBoxes)
                {
                    recurrenceOk |= c.Checked;
                }
            }
            saveButton.Enabled = recurrenceOk && HasUnsavedChanges && nameText.Text != "";
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

        /*
         * Determines whether an IME is open. If one is, then the save button
         * is hidden. Otherwise, the save button is revealed.
         */
        private void CheckIME()
        {
            int heightDiff = contentLayout.RootView.Height - contentLayout.Height;
            if (heightDiff > this.DPToNearestPX(200))
            {
                saveButton.Visibility = ViewStates.Gone;
            }
            else
            {
                saveButton.Visibility = ViewStates.Visible;
            }
        }
    }
}