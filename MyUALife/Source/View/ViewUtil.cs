using Android.Content;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.App;
using Android.Views;
using System;
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
         * Creates a TextView as part of this ViewUtil's context that contains
         * the specified text. The background of the TextView will be a rounded
         * rectangle of the specified color.
         */
        public TextView GenerateTextView(String text, String colorString)
        {
            // Create the text view
            TextView view = new TextView(ContextWrapper);

            // Add 6dp of padding on the left and right
            int padding = DPToNearestPX(6f);
            view.SetPadding(view.PaddingLeft + padding, view.PaddingTop, view.PaddingRight + padding, view.PaddingBottom);

            // Create a LayoutParams to set the margins
            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
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
            sd.Paint.Color = Color.ParseColor(colorString);
            view.Background = sd;

            // Set the text
            view.Text = text;

            // Set the text to white
            view.SetTextColor(Color.White);
            return view;
        }

        /*
         * Fills the supplied layout with TextViews representing the events in
         * events. Views are added in the order that the events appear in the
         * list. When the views are long clicked, an Alertdialog is opened
         * asking the user if they want to edit or delete the event.
         */
        public void LoadEventsToLayout(LinearLayout layout, List<Event> events)
        {
            layout.RemoveAllViews();
            foreach (Event e in events)
            {
                TextView view = GenerateTextView(e.ToString(), e.Type.colorString);

                // Register an event handler to delete or edit the event
                view.LongClick += (sender, ea) =>
                {
                    var infoDialog = new AlertDialog.Builder(ContextWrapper);
                    infoDialog.SetMessage("Delete or edit this event?");
                    infoDialog.SetPositiveButton("Delete", delegate
                    {
                        layout.RemoveView(view);
                        Model.Calendar.RemoveEvent(e);
                    });
                    if (ContextWrapper is NewMainActivity)
                    {
                        infoDialog.SetNeutralButton("Edit", delegate
                        {
                            ((NewMainActivity) ContextWrapper).StartEditEventActivity(e);
                        });
                    }
                    infoDialog.SetNegativeButton("Cancel", delegate { });
                    infoDialog.Show();
                };

                layout.AddView(view);
            }
        }

        /*
         * Fills the supplied layout with TextViews representing the deadlines in
         * deadlines. Views are added in the order that the deadlines appear in the
         * list.
         */
        public void LoadDeadlinesToLayout(LinearLayout layout, List<Deadline> deadlines)
        {
            layout.RemoveAllViews();
            foreach (Deadline d in deadlines)
            {
                TextView view = GenerateTextView(d.ToString(), "#F44336");
                // TODO: long click?
                layout.AddView(view);
            }
        }

        /*
         * Fills the supplied layout with TextViews that are designed to
         * represent a block of free time. Views are added in the order that
         * the free time events appear in events.
         */
        public void LoadFreeTimeToLayout(LinearLayout layout, List<Event> events)
        {
            layout.RemoveAllViews();
            foreach (Event e in events)
            {
                String format = "{0} - {1}";
                String text = String.Format(format, e.StartTime, e.EndTime);
                TextView view = GenerateTextView(text, e.Type.colorString);
                layout.AddView(view);
            }
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
    }
}