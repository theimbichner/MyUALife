using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using System;

namespace MyUALife
{
    [Activity(Label = "Create Deadline")]
    public class DeadlineEditorActivity : Activity
    {
        // GUI components
        private EditText nameText;
        private EditText descriptionText;
        private TextView timeLabel;
        private Button changeTimeButton;
        private Button saveButton;

        // The times selected with DatePickerFragment.
        private DateTimeFetcher deadlineTime;

        // Helper for the type spinner
        private SpinnerHelper<EventType> typeSpinner;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.DeadlineEditor);

            // Get components from id
            nameText = FindViewById<EditText>(Resource.Id.nameText);
            descriptionText = FindViewById<EditText>(Resource.Id.descriptionText);
            timeLabel = FindViewById<TextView>(Resource.Id.timeLabel);
            changeTimeButton = FindViewById<Button>(Resource.Id.changeTimeButton);
            saveButton = FindViewById<Button>(Resource.Id.saveButton);

            // Setup the text fields to turn on the save button when edited
            nameText.TextChanged += (sender, e) => TurnOnSaveButton();
            descriptionText.TextChanged += (sender, e) => TurnOnSaveButton();

            // Configure the description text to display correctly
            descriptionText.SetHorizontallyScrolling(false);
            descriptionText.SetMaxLines(1000);

            // Setup the time buttons to display a time picker dialog
            deadlineTime = new DateTimeFetcher(this, DateTime.Now, OnTimeChanged);
            changeTimeButton.Click += (sender, e) => deadlineTime.PollDateTime(true);

            // Setup the save changes button to save the current data when pressed
            saveButton.Click += (sender, e) => SaveChanges();

            // Disable the save changes button by default
            saveButton.Enabled = false;

            // Configure the spinner to display the correct list of EventTypes
            Spinner spinner = FindViewById<Spinner>(Resource.Id.typeSpinner);
            typeSpinner = new SpinnerHelper<EventType>(spinner, Category.CreatableTypes, t => t.Name);

            // Setup the spinner to turn on the save button
            typeSpinner.Spinner.ItemSelected += (sender, e) => TurnOnSaveButton();

            // Get the deadline stored in Intent, if any
            Deadline input = Deadline.ReadDeadline(Intent, MainActivity.InputDeadline);
            if (input != null)
            {
                // Store data from input in the components
                nameText.Text = input.Name;
                descriptionText.Text = input.Description;
                deadlineTime.Time = input.Time;
                typeSpinner.SelectedItem = input.Type;
                SaveChanges();
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Initialize the start/end time labels with the correct times
            UpdateTimeLabel();
        }

        /*
         * Transfers the data entered into the GUI elements into an Intent
         * extra and returns the intent to the main activity.
         */
        private void SaveChanges()
        {
            EventType type = typeSpinner.SelectedItem;
            Deadline result = new Deadline(nameText.Text, descriptionText.Text, deadlineTime.Time, type);
            Intent data = new Intent();
            Deadline.WriteDeadline(data, MainActivity.ResultDeadline, result);
            SetResult(Result.Ok, data);
            saveButton.Enabled = false;
        }

        /*
         * Updates the time labels that display the selected start and end
         * times to reflect the DateTimes selected by the user.
         */
        private void UpdateTimeLabel()
        {
            timeLabel.Text = deadlineTime.Time.ToString("D");
            timeLabel.Text += "\n" + deadlineTime.Time.ToString("t");
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
            saveButton.Enabled = true;
        }

        /*
         * This method should be called whenever the user uses the time changer
         * button to set the end time for the deadline
         */
        private void OnTimeChanged()
        {
            UpdateTimeLabel();
            TurnOnSaveButton();
        }
    }
}