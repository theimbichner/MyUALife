using Android.App;
using Android.Widget;
using Android.OS;
using Android.Provider;
using Android.Content;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MyUALife
{
    [Activity(Label = "MyUALife", MainLauncher = true)]
    public class TitleScreenActivity : Activity
    {
        // Request codes for the EventEditorActivity
        private const int studentLoginRequest = 1;
        private const int staffLoginRequest = 2;
        
        // GUI components
        private Button studentButton;
        private Button staffButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.TitleScreen);

            studentButton = FindViewById<Button>(Resource.Id.studentLoginButton);
            staffButton = FindViewById<Button>(Resource.Id.staffLoginButton);

            studentButton.Click += (sender, e) => StartMainActivity();
            staffButton.Click += (sender, e) => StartMainActivity();
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode != Result.Ok)
            {
                // If the activity terminated abnormally, do not attempt to add anything
                return;
            }
        }

        private void StartMainActivity()
        {
            Intent intent = new Intent(this, typeof(MainActivity));
            StartActivityForResult(intent, studentLoginRequest);
        }
    }
}