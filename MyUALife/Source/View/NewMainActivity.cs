using Android.App;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using System;
using System.Collections.Generic;

namespace MyUALife
{
    [Activity(Label = "MyUALife", MainLauncher = true)]
    public class NewMainActivity : Activity
    {
        private Color darkBlue;
        private Color lightBlue;
        private Color white;

        private DateTime loadedDate;
        private List<Event> loadedEvents;
        private Color currentColor;

        // GUI components
        private Button filterButton;
        private Button calendarButton;
        private Button happeningsButton;
        private Button createEventButton;
        private Button createDeadlineButton;
        private LinearLayout mainTextLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.NewMain);

            // Get components by id
            filterButton = FindViewById<Button>(Resource.Id.filterButton);
            calendarButton = FindViewById<Button>(Resource.Id.calendarButton);
            happeningsButton = FindViewById<Button>(Resource.Id.happeningsButton);
            createEventButton = FindViewById<Button>(Resource.Id.createEventButton);
            createDeadlineButton = FindViewById<Button>(Resource.Id.createDeadlineButton);
            mainTextLayout = FindViewById<LinearLayout>(Resource.Id.mainTextLayout);

            // Get colors by id
            darkBlue = Resources.GetColor(Resource.Color.indigo700, Theme);
            lightBlue = Resources.GetColor(Resource.Color.indigo500, Theme);
            white = Resources.GetColor(Resource.Color.white, Theme);

            // Setup the filter button to filter events
            filterButton.Click += (sender, e) =>
            {

            };

            // Setup the calendar button to open the android calendar
            calendarButton.Click += (sender, e) =>
            {
                Android.Net.Uri.Builder builder = CalendarContract.ContentUri.BuildUpon();
                builder.AppendPath("time");
                long millis = (long)(DateTime.Today - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                ContentUris.AppendId(builder, millis);
                Intent intent = new Intent(Intent.ActionView).SetData(builder.Build());
                StartActivity(intent);
            };

            // Setup the happenings button to display a list of happenings
            happeningsButton.Click += (sender, e) =>
            {

            };

            // Setup the create event button to open the create event screen
            createEventButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(EventViewActivity));
                StartActivity(intent);
            };

            // Setup the deadline button to open the create deadline screen
            createDeadlineButton.Click += (sender, e) =>
            {

            };
        }

        protected override void OnStart()
        {
            base.OnStart();

            // Load the events scheduled for today
            LoadEvents();
        }

        /*
         * Loads every event scheduled for the current day into the main text
         * view.
         */
        private void LoadEvents()
        {
            // Load events from today
            loadedDate = DateTime.Today;

            // Create a range of DateTimes
            // We want to count midnight as belonging to the previous day.
            // Hence, we start our range just after midnight
            DateTime start = loadedDate.AddMilliseconds(1);
            DateTime end = loadedDate.AddDays(1);

            // Get the events in range from the calendar
            loadedEvents = Model.Calendar.GetEventsInRange(start, end);

            // Clear the main layout of text views
            mainTextLayout.RemoveAllViews();

            // Add a new text view for every event
            currentColor = darkBlue;
            foreach (Event e in loadedEvents)
            {
                mainTextLayout.AddView(GenerateTextView(e));

                // Toggle the color for the next button
                // Results in an alternating red-blue pattern
                ToggleCurrentColor();
            }
        }

        /*
         * Returns a new TextView that displays data about the given event.
         * When the returned TextView is long clicked, an AlertDialog is opened
         * asking the user if they want to edit the event.
         */
        private TextView GenerateTextView(Event calEvent)
        {
            // Create the text view and set its text
            TextView view = new TextView(this);
            view.Text = calEvent.ToString();

            float density = this.Resources.DisplayMetrics.Density;

            // Add 6dp of padding on the left and right
            float dpPadding = 6f;
            int pxPadding = (int) (dpPadding * density + 0.5f);
            view.SetPadding(view.PaddingLeft + pxPadding, view.PaddingTop, view.PaddingRight + pxPadding, view.PaddingBottom);

            // Add a 2dp margin below
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            float dpMarginBottom = 2f;
            int pxMarginBottom = (int) (dpMarginBottom * density + 0.5f);
            layoutParams.BottomMargin = pxMarginBottom;

            // Add 3dp margins to the left/right
            float dpMarginLR = 3f;
            int pxMarginLR = (int)(dpMarginLR * density + 0.5f);
            layoutParams.LeftMargin = pxMarginLR;
            layoutParams.RightMargin = pxMarginLR;

            view.LayoutParameters = layoutParams;

            // Set the background to a rounded rectangle
            float dpRadius = 4f;
            float pxRadius = dpRadius * density;
            float[] radii = new float[8];
            for (int i = 0; i < 8; i++)
            {
                radii[i] = pxRadius;
            }
            Shape s = new RoundRectShape(radii, null, null);
            ShapeDrawable sd = new ShapeDrawable(s);

            // Set the color
            sd.Paint.Color = currentColor;
            view.Background = sd;

            // Set the text to white
            view.SetTextColor(white);

            // Register the event handler to edit the event
            view.LongClick += (sender, e) =>
            {
                var infoDialog = new AlertDialog.Builder(this);
                infoDialog.SetMessage("Edit this event?");
                infoDialog.SetPositiveButton("Edit", delegate
                {
                    // Inform the user that this feature is not implemented
                    var notYetImplementedDialog = new AlertDialog.Builder(this);
                    notYetImplementedDialog.SetMessage("Not yet implemented");
                    notYetImplementedDialog.SetPositiveButton("Ok", delegate { });
                    notYetImplementedDialog.Show();
                });
                infoDialog.SetNegativeButton("Cancel", delegate { });
                infoDialog.Show();
            };
            return view;
        }

        /*
         * Toggles the color of the next generated button between blue and red.
         */
        private void ToggleCurrentColor()
        {
            if (currentColor == darkBlue)
            {
                currentColor = lightBlue;
            }
            else
            {
                currentColor = darkBlue;
            }
        }
    }
}

