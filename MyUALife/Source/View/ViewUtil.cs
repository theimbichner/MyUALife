using Android.Content;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.App;
using System.Collections.Generic;

namespace MyUALife
{
    class ViewUtil
    {
        public ContextWrapper ContextWrapper
        {
            get;
            private set;
        }

        /*
         * Creates a ViewUtil object to support the given Context.
         */
        public ViewUtil(ContextWrapper cw)
        {
            ContextWrapper = cw;
        }

        /*
         * Converts a given length in dp to px.
         */
        public float DPToPX(float dp)
        {
            float density = ContextWrapper.Resources.DisplayMetrics.Density;
            return density * dp;
        }

        /*
         * Converts a given length in dp to px. Rounds to the nearest int.
         */
        public int DPToNearestPX(float dp)
        {
            float density = ContextWrapper.Resources.DisplayMetrics.Density;
            return (int)(dp * density + 0.5f);
        }

        /*
         * Returns a new TextView that displays data about the given event.
         * When the returned TextView is long clicked, an AlertDialog is opened
         * asking the user if they want to edit the event. The view's
         * background is a rounded rectangle with the given color.
         */
        public TextView GenerateTextView(Event calendarEvent)
        {
            // Create the text view and set its text
            TextView view = new TextView(ContextWrapper);
            view.Text = calendarEvent.ToString();

            // Add 6dp of padding on the left and right
            int padding = DPToNearestPX(6f);
            view.SetPadding(view.PaddingLeft + padding, view.PaddingTop, view.PaddingRight + padding, view.PaddingBottom);

            // Create a LayoutParams to set the margins
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent);
            view.LayoutParameters = layoutParams;

            // Add a 2dp margin below
            layoutParams.BottomMargin = DPToNearestPX(3f);

            // Add 3dp margins to the left/right
            int margin = DPToNearestPX(5f);
            layoutParams.LeftMargin = margin;
            layoutParams.RightMargin = margin;

            // Set the background to be a rounded rectangle
            float radius = DPToPX(4f);
            float[] radii = new float[8];
            for (int i = 0; i < 8; i++)
            {
                radii[i] = radius;
            }
            Shape s = new RoundRectShape(radii, null, null);
            ShapeDrawable sd = new ShapeDrawable(s);
            view.Background = sd;

            // Set the background color
            sd.Paint.Color = Color.ParseColor(calendarEvent.Type.colorString);

            // Set the text to white
            view.SetTextColor(Color.White);
            return view;
        }

        public void LoadEventsToLayout(LinearLayout layout, List<Event> events)
        {
            layout.RemoveAllViews();
            foreach (Event e in events)
            {
                TextView view = GenerateTextView(e);

                // Register an event handler to or deleteedit the event
                view.LongClick += (sender, ea) =>
                {
                    var infoDialog = new AlertDialog.Builder(ContextWrapper);
                    infoDialog.SetMessage("Delete or edit this event?");
                    infoDialog.SetPositiveButton("Delete", delegate
                    {
                        layout.RemoveView(view);
                        Model.Calendar.RemoveEvent(e);
                    });
                    infoDialog.SetNeutralButton("Edit", delegate
                    {
                        // Inform the user that this feature is not implemented
                        var notYetImplementedDialog = new AlertDialog.Builder(ContextWrapper);
                        notYetImplementedDialog.SetMessage("Not yet implemented");
                        notYetImplementedDialog.SetPositiveButton("Ok", delegate { });
                        notYetImplementedDialog.Show();
                    });
                    infoDialog.SetNegativeButton("Cancel", delegate { });
                    infoDialog.Show();
                };

                layout.AddView(view);
            }
        }
    }
}