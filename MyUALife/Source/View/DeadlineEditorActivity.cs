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
        private DateTime deadlineTime = DateTime.Now;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.EventEditor);

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
            changeTimeButton.Click += (sender, e) => PollDateTime();

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
                deadlineTime = input.Time;
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
            Deadline result = new Deadline(nameText.Text, descriptionText.Text, deadlineTime, Category.GetTypeByName(typeName));
            Intent data = new Intent();
            new DeadlineSerializer(data).WriteDeadline(DeadlineSerializer.ResultDeadline, result);
            SetResult(Result.Ok, data);
            saveButton.Enabled = false;
        }

        /*
         * A DialogFragment that shows the user a DatePickerDialog and passes
         * the picked date along to a TimePickerFragment.
         */
        private class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
        {

            public DatePickerFragment()
            {
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                DateTime date = DateTime.Today;
                return new DatePickerDialog(Activity, this, date.Year, date.Month - 1, date.Day);
            }

            public void OnDateSet(DatePicker view, int year, int month, int day)
            {
                DateTime date = new DateTime(year, month + 1, day);
                TimePickerFragment timePicker = new TimePickerFragment(date);
                timePicker.Show(FragmentManager, "pickStartTime");
            }
        }

        /*
         * A DialogFragment that takes a date, gets a time from the user via a
         * TimePickerDialog, and creates a DateTime from this information. This
         * DateTime is then stored in eventTimes.
         */
        private class TimePickerFragment : DialogFragment, TimePickerDialog.IOnTimeSetListener
        {
            private readonly DateTime date;

            public TimePickerFragment(DateTime date)
            {
                this.date = date;
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                DateTime time = DateTime.Now;
                return new TimePickerDialog(Activity, this, time.Hour, time.Minute, false);
            }

            public void OnTimeSet(TimePicker view, int hour, int minute)
            {
                DeadlineEditorActivity deadlineEditor = (DeadlineEditorActivity)Activity;
                deadlineEditor.deadlineTime = date.AddHours(hour).AddMinutes(minute);
                deadlineEditor.UpdateTimeLabels();
                deadlineEditor.TurnOnSaveButton();
            }
        }

        /*
         * Updates the time labels that display the selected start and end
         * times to reflect the DateTimes selected by the user.
         */
        private void UpdateTimeLabels()
        {
            timeLabel.Text = deadlineTime.ToString("g");
        }

        /*
         * Opens a dialog asking the user for a time and stores the result in
         * the appropriate entry in eventTimes.
         */
        private void PollDateTime()
        {
            DatePickerFragment dateTimePicker = new DatePickerFragment();
            dateTimePicker.Show(FragmentManager, "pickStartDateTime");
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
    }
}