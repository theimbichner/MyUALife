using System;
using Android.App;
using Android.Widget;
using Android.OS;

namespace MyUALife
{
    public class DateTimeFetcher
    {
        public delegate void CompletionCallback();

        public DateTime Time
        {
            get;
            set;
        }
        private readonly CompletionCallback callback;
        private readonly Activity activity;

        /*
         * Creates a DateTimeFetcher for the specified activity, initialized to
         * store the given DateTime. Calls callback once the time has been set
         * via dialog.
         */
        public DateTimeFetcher(Activity activity, DateTime initTime, CompletionCallback callback)
        {
            this.activity = activity;
            Time = initTime;
            this.callback = callback;
        }

        /*
         * A DialogFragment that shows the user a DatePickerDialog and passes
         * the picked date along to a TimePickerFragment.
         */
        private class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
        {
            private readonly DateTimeFetcher parent;

            public DatePickerFragment(DateTimeFetcher parent)
            {
                this.parent = parent;
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                int year = parent.Time.Year;
                int month = parent.Time.Month - 1;
                int day = parent.Time.Day;
                return new DatePickerDialog(Activity, this, year, month, day);
            }

            public void OnDateSet(DatePicker view, int year, int month, int day)
            {
                DateTime date = new DateTime(year, month + 1, day);
                TimePickerFragment timePicker = new TimePickerFragment(parent, date);
                timePicker.Show(FragmentManager, "pickStartTime");
            }
        }

        /*
         * A DialogFragment that takes a date, gets a time from the user via a
         * TimePickerDialog, and creates a DateTime from this information. This
         * DateTime is then stored in the associated DateTimeFetcher.
         */
        private class TimePickerFragment : DialogFragment, TimePickerDialog.IOnTimeSetListener
        {
            private readonly DateTimeFetcher parent;
            private readonly DateTime date;

            public TimePickerFragment(DateTimeFetcher parent, DateTime date)
            {
                this.parent = parent;
                this.date = date;
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                int hour = parent.Time.Hour;
                int min = parent.Time.Minute;
                return new TimePickerDialog(Activity, this, hour, min, false);
            }

            public void OnTimeSet(TimePicker view, int hour, int minute)
            {
                parent.Time = date.AddHours(hour).AddMinutes(minute);
                parent.callback?.Invoke();
            }
        }

        /*
         * Opens a dialog asking the user for a time and stores the result as a
         * DateTime once the dialog has closed, if it closes successfully.
         */
        public void PollDateTime(bool useDate)
        {
            if (useDate)
            {
                DatePickerFragment dateTimePicker = new DatePickerFragment(this);
                dateTimePicker.Show(activity.FragmentManager, "pickStartDateTime");
            }
            else
            {
                TimePickerFragment timePicker = new TimePickerFragment(this, DateTime.Today);
                timePicker.Show(activity.FragmentManager, "pickStartTime");
            }
        }
    }
}