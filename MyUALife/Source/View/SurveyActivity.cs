using Android.App;
using Android.OS;
using Android.Widget;

namespace MyUALife
{
    [Activity(Label = "Survey")]
    public class SurveyActivity : Activity
    {
        // GUI components
        private TextView nameLabel;
        private TextView descriptionLabel;
        private TextView startTimeLabel;
        private TextView endTimeLabel;
        private TextView estimateLabel;
        private EditText timeText;
        private Button submitButton;
        private Button ignoreButton;

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

            /*
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

            */
        }
    }
}