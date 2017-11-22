using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using Android.Support.V4.Widget;
using System;
using System.Collections.Generic;

namespace MyUALife
{
    [Activity(Label = "Survey")]
    public class SurveyActivity : Activity
    {
        /*
         * Currently this file is mostly just a copy of EventEditorActivity.cs, with a few changes to the variables
         * - Daniel
         */


        // GUI components
        private TextView nameLabel;
        private TextView descriptionLabel;
        private TextView startTimeLabel;
        private TextView endTimeLabel;
        private TextView estimateLabel;
        private EditText timeText;
        private Button submitButton;
        private Button ignoreButton;

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
            }
        }

        // True when the drawer is open
        private bool drawerOpen = false;
        private bool DrawerOpen
        {
            get
            {
                return drawerOpen;
            }
            set
            {
                drawerOpen = value;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.Survey);

            // Get components from id
            nameLabel = FindViewById<TextView>(Resource.Id.nameLabel);
            descriptionLabel = FindViewById<TextView>(Resource.Id.descriptionLabel);
            startTimeLabel = FindViewById<TextView>(Resource.Id.startTimeLabel);
            endTimeLabel = FindViewById<TextView>(Resource.Id.endTimeLabel);
            estimateLabel = FindViewById<TextView>(Resource.Id.estimateLabel);
            timeText = FindViewById<EditText>(Resource.Id.timeText);
            submitButton = FindViewById<Button>(Resource.Id.submitButton);
            ignoreButton = FindViewById<Button>(Resource.Id.ignoreButton);
            
            // Configure the description text to display correctly
            descriptionLabel.SetHorizontallyScrolling(false);
            descriptionLabel.SetMaxLines(1000);

            // Get the event stored in Intent, if any
            Event input = new EventSerializer(Intent).ReadEvent(EventSerializer.InputEvent);
            if (input != null)
            {
                // Store data from input in the components
                nameLabel.Text = input.Name;
                descriptionLabel.Text = input.Description;
                SaveChanges();
            }
            else
            {
                Deadline deadline = new DeadlineSerializer(Intent).ReadDeadline(DeadlineSerializer.InputDeadline);
                if (deadline != null)
                {
                    nameLabel.Text = deadline.Name;
                    descriptionLabel.Text = deadline.Description;

                    Intent returnData = new Intent();
                    new DeadlineSerializer(returnData).WriteDeadline(DeadlineSerializer.ResultDeadline, deadline);
                    SetResult(Result.Ok, returnData);
                }
            }

            // Load the free time blocks from the intent
            EventSerializer deserializer = new EventSerializer(Intent);
            List<Event> pastEvents = new List<Event>();
            int count = Intent.GetIntExtra("PastEventCount", 0);
            for (int i = 0; i < count; i++)
            {
                Event pastEvent = deserializer.ReadEvent("PastEvent" + i);
                if (pastEvent != null)
                {
                    pastEvents.Add(pastEvent);
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
                    
                };
            };
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        /*
         * Transfers the data entered into the GUI elements into the fields of
         * the current Event. If there is no current Event, this method creates
         * one and stores it in the Calendar. Any additional data that needs to
         * be returned to the caller can be stored in the input intent.
         */
        private void SaveChanges(Intent data)
        {
            //Event resultEvent = new Event(nameLabel.Text, descriptionLabel.Text, type, startTime.Time, endTime.Time);
            //new EventSerializer(data).WriteEvent(EventSerializer.ResultEvent, resultEvent);
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
    }
}