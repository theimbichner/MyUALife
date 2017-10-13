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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set view from layout resource
            SetContentView(Resource.Layout.EventView);

            // Get components from id
            Button backButton = FindViewById<Button>(Resource.Id.backButton);
            EditText descriptionText = FindViewById<EditText>(Resource.Id.descriptionText);

            // Configure the description text to display correctly
            descriptionText.SetHorizontallyScrolling(false);
            descriptionText.SetMaxLines(1000);

            // Setup the back button to return to the main activity
            backButton.Click += (sender, e) =>
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };
        }
    }
}