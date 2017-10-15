using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MyUALife
{
    [Activity(Label = "MyUALife")]
    public class EventViewActivity : Activity
    {
        // GUI components
        Button saveEditButton;
        Button changeStartButton, changeEndButton;
        Button backButton;
        EditText descriptionText, nameText;
        TextView startTimeLabel, endTimeLabel;

        // The selected event
        private Event selectedEvent = null;

        // Currently in edit mode?
        private Boolean editMode = true;
        private Boolean EditMode
        {
            get
            {
                return editMode;
            }

            set
            {
                editMode = value;
                nameText.Enabled = value;
                descriptionText.Enabled = value;
                changeStartButton.Enabled = value;
                changeEndButton.Enabled = value;
                saveEditButton.Text = value ? "Save Changes" : "Edit Event";
            }
        }

        // The times selected with TimePickerFragment.
        private DateTime[] eventTimes = {DateTime.Now, DateTime.Now};
        private readonly int StartTimeIndex = 0;
        private readonly int EndTimeIndex = 1;
        private DateTime StartTime
        {
            get
            {
                return eventTimes[StartTimeIndex];
            }

            set
            {
                eventTimes[StartTimeIndex] = value;
            }
        }
        private DateTime EndTime
        {
            get
            {
                return eventTimes[EndTimeIndex];
            }

            set
            {
                eventTimes[EndTimeIndex] = value;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.EventView);

            // Get components from id
            saveEditButton = FindViewById<Button>(Resource.Id.saveEditButton);
            backButton = FindViewById<Button>(Resource.Id.backButton);

            descriptionText = FindViewById<EditText>(Resource.Id.descriptionText);
            nameText = FindViewById<EditText>(Resource.Id.nameText);

            changeStartButton = FindViewById<Button>(Resource.Id.changeStartButton);
            changeEndButton = FindViewById<Button>(Resource.Id.changeEndButton);

            startTimeLabel = FindViewById<TextView>(Resource.Id.startTimeLabel);
            endTimeLabel = FindViewById<TextView>(Resource.Id.endTimeLabel);

            // Pull the selected Event from Intent

            // Configure the description text to display correctly
            descriptionText.SetHorizontallyScrolling(false);
            descriptionText.SetMaxLines(1000);

            // Initialize the start/end time labels
            updateTimeLabels();

            // Setup the back button to return to the main activity
            backButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };

            // Setup the start time button to display a time picker dialog
            changeStartButton.Click += (sender, e) => pollDateTime(StartTimeIndex);

            // Setup the end time button to display a time picker dialog
            changeEndButton.Click += (sender, e) => pollDateTime(EndTimeIndex);

            // Setup the save changes/edit event button
            saveEditButton.Click += (sender, e) =>
            {
                if (EditMode)
                {
                    saveChanges();
                }
                EditMode = !EditMode;
            };
        }

        /*
         * Transfers the data entered into the GUI elements into the fields of
         * the current Event. If there is no current Event, this method creates
         * one and stores it in the Calendar.
         */
        private void saveChanges()
        {
            if (selectedEvent == null)
            {
                selectedEvent = new Event(nameText.Text, descriptionText.Text, Category.recreation, StartTime, EndTime);
                Model.getCalendar().AddEvent(selectedEvent);
            }
            else
            {
                selectedEvent.Name = nameText.Text;
                selectedEvent.Description = descriptionText.Text;
                selectedEvent.StartTime = StartTime;
                selectedEvent.EndTime = EndTime;
            }
        }

        /*
         * A DialogFragment that shows the user a DatePickerDialog and passes
         * the picked along to a TimePickerFragment.
         */
        private class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
        {
            private readonly int timeIndex;

            public DatePickerFragment(int timeIndex)
            {
                this.timeIndex = timeIndex;
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                DateTime date = DateTime.Today;
                return new DatePickerDialog(this.Activity, this, date.Year, date.Month - 1, date.Day);
            }

            public void OnDateSet(DatePicker view, int year, int month, int day)
            {
                DateTime date = new DateTime(year, month + 1, day);
                TimePickerFragment timePicker = new TimePickerFragment(date, timeIndex);
                timePicker.Show(this.FragmentManager, "pickStartTime");
            }
        }

        /*
         * A DialogFragment that takes a date, gets a time from the user via a
         * TimePickerDialog and creates a DateTime from this information. This
         * DateTime is then stored in eventTimes.
         */
        private class TimePickerFragment : DialogFragment, TimePickerDialog.IOnTimeSetListener
        {
            private readonly int timeIndex;
            private readonly DateTime date;

            public TimePickerFragment(DateTime date, int timeIndex)
            {
                this.timeIndex = timeIndex;
                this.date = date;
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                DateTime time = DateTime.Now;
                return new TimePickerDialog(this.Activity, this, time.Hour, time.Minute, false);
            }

            public void OnTimeSet(TimePicker view, int hour, int minute)
            {
                EventViewActivity parent = (EventViewActivity) this.Activity;
                parent.eventTimes[timeIndex] = date.AddHours(hour).AddMinutes(minute);
                parent.updateTimeLabels();
            }
        }

        /*
         * Updates the time labels that display the selected start and end
         * times to reflect the DateTimes selected by the user.
         */
        private void updateTimeLabels()
        {
            startTimeLabel.Text = StartTime.ToString("g");
            endTimeLabel.Text = EndTime.ToString("g");
        }

        /*
         * Opens a dialog asking the user for a time and stores the result in
         * the appropriate entry in eventTimes.
         */
        private void pollDateTime(int timeIndex)
        {
            DatePickerFragment dateTimePicker = new DatePickerFragment(timeIndex);
            dateTimePicker.Show(this.FragmentManager, "pickStartDateTime");
        }
    }
}