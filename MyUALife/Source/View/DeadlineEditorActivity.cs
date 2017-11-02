using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using System;
using System.Collections.Generic;

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
        private Spinner typeSpinner;

        // The times selected with DatePickerFragment.
        private DateTimeFetcher deadlineTime;

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
            typeSpinner = FindViewById<Spinner>(Resource.Id.typeSpinner);

            // Setup the text fields to turn on the save button when edited
            nameText.TextChanged += (sender, e) => TurnOnSaveButton();
            descriptionText.TextChanged += (sender, e) => TurnOnSaveButton();

            // Configure the description text to display correctly
            descriptionText.SetHorizontallyScrolling(false);
            descriptionText.SetMaxLines(1000);

            // Setup the time buttons to display a time picker dialog
            deadlineTime = new DateTimeFetcher(this, DateTime.Now, OnTimeChanged);
            changeTimeButton.Click += (sender, e) => deadlineTime.PollDateTime();

            // Setup the save changes button to save the current data when pressed
            saveButton.Click += (sender, e) => SaveChanges();

            // Disable the save changes button by default
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

            // Get the DEADLINE stored in Intent, if any
            Deadline input = new DeadlineSerializer(Intent).ReadDeadline(DeadlineSerializer.InputDeadline);
            if (input != null)
            {
                // Store data from input in the components
                nameText.Text = input.Name;
                descriptionText.Text = input.Description;
                deadlineTime.Time = input.Time;
                typeSpinner.SetSelection(adapter.GetPosition(input.associatedEventType.name));
                SaveChanges();
            }

            // Initialize the start/end time labels with the correct times
            UpdateTimeLabels();
        }

        /*
         * Transfers the data entered into the GUI elements into the fields of
         * the current Event. If there is no current Event, this method creates
         * one and stores it in the Calendar.
         */
        private void SaveChanges()
        {
            String typeName = typeSpinner.SelectedItem.ToString();
            Deadline result = new Deadline(nameText.Text, descriptionText.Text, deadlineTime.Time, Category.GetTypeByName(typeName));
            Intent data = new Intent();
            new DeadlineSerializer(data).WriteDeadline(DeadlineSerializer.ResultDeadline, result);
            SetResult(Result.Ok, data);
            saveButton.Enabled = false;
        }

        /*
         * Updates the time labels that display the selected start and end
         * times to reflect the DateTimes selected by the user.
         */
        private void UpdateTimeLabels()
        {
            timeLabel.Text = deadlineTime.Time.ToString("g");
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
            UpdateTimeLabels();
            TurnOnSaveButton();
        }
    }
}