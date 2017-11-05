using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Provider;
using Android.Database;
using Android.Content;

namespace MyUALife
{
    [Activity(Label = "MyUALife")]
    public class MainActivity : Activity, CalendarView.IOnDateChangeListener
    {
        private int selectedDay = DateTime.Now.Day;
        private int selectedMonth = DateTime.Now.Month;
        private int selectedYear = DateTime.Now.Year;

        // GUI components
        Button openDayViewButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.Main);

            // Get components by id
            CalendarView mainCalendar = FindViewById<CalendarView>(Resource.Id.mainCalendar);
            openDayViewButton = FindViewById<Button>(Resource.Id.openDayViewButton);
            Button addEventButton = FindViewById<Button>(Resource.Id.addEventButton);
            Button externalCalendarButton = FindViewById<Button>(Resource.Id.externalCalendarButton);

            // Create dateFetcher to read the current date from the calendar display
            mainCalendar.SetOnDateChangeListener(this);

            // Setup the button to open a new view when clicked.
            openDayViewButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(DayViewActivity));
                intent.PutExtra("date", new int[] {selectedDay, selectedMonth, selectedYear});
                StartActivity(intent);
            };

            // Initialize the text for the button
            updateButtonText();

            // Setup the add event button to open the new event view when clicked
            addEventButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(EventEditorActivity));
                StartActivity(intent);
            };

            // Setup the goto calendar button to open the android calendar app
            externalCalendarButton.Click += (sender, e) =>
            {
                Android.Net.Uri.Builder builder = CalendarContract.ContentUri.BuildUpon();
                builder.AppendPath("time");
                ContentUris.AppendId(builder, (long) (DateTime.Today - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
                Intent intent = new Intent(Intent.ActionView).SetData(builder.Build());
                StartActivity(intent);
            };
        }

        public void OnSelectedDayChange(CalendarView view, int year, int month, int day)
        {
            selectedDay = day;
            selectedYear = year;
            selectedMonth = month + 1; // convert from 0-start to 1-start
            updateButtonText();
        }

        private void updateButtonText()
        {
            openDayViewButton.Text = String.Format("View events for {0}/{1}/{2}", selectedMonth, selectedDay, selectedYear);
        }

        private void queryAllEvents(long startMillis, long endMillis)
        {
            String[] InstanceProjection = new String[] {CalendarContract.Instances.EventId,
                                                        CalendarContract.Instances.Begin,
                                                        CalendarContract.Instances.End,
                                                        CalendarContract.EventsColumns.Title};
            ContentResolver cr = ContentResolver;

            String selection = ""; // TODO Is this how we get all the instances?
            String[] selectionArgs = {""};
            Android.Net.Uri.Builder builder = CalendarContract.Instances.ContentUri.BuildUpon();
            ContentUris.AppendId(builder, startMillis);
            ContentUris.AppendId(builder, endMillis);

            ICursor cur = cr.Query(builder.Build(), InstanceProjection, selection, selectionArgs, null);

            while (cur.MoveToNext())
            {
                long eventId = cur.GetLong(0);
                long begin = cur.GetLong(1);
                long end = cur.GetLong(2);
                String title = cur.GetString(3);

                // do stuff with this information
            }
        }
    }
}

