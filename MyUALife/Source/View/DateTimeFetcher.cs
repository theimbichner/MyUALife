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
                DateTime date = DateTime.Today;
                return new DatePickerDialog(Activity, this, date.Year, date.Month - 1, date.Day);
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
                DateTime time = DateTime.Now;
                return new TimePickerDialog(Activity, this, time.Hour, time.Minute, false);
            }

            public void OnTimeSet(TimePicker view, int hour, int minute)
            {
                parent.Time = date.AddHours(hour).AddMinutes(minute);
                parent.callback();
            }
        }

        /*
         * Opens a dialog asking the user for a time and stores the result as a
         * DateTime once the dialog has closed, if it closes successfully.
         */
        public void PollDateTime()
        {
            DatePickerFragment dateTimePicker = new DatePickerFragment(this);
            dateTimePicker.Show(activity.FragmentManager, "pickStartDateTime");
        }
    }
}