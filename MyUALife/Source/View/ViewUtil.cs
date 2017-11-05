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
        // Function for converting a generic object into a string
        public delegate String ToStr<T>(T t);

        // Function to perform setup operations on a textView
        public delegate void SetupCallback<T>(TextView textView, LinearLayout layout, T target);

        // The ContextWrapper that this util supports
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
         * Fills the supplied layout with TextViews representing the objects in
         * objs. Views are added in the order that objects appear in the list.
         * The text displayed in the TextView is given by label(obj). The
         * background of the TextView is a rounded rectangle with the color
         * represented by color(obj). Each TextView is setup with setup before
         * it is added to the layout.
         */
        public void LoadListToLayout<T>(LinearLayout layout, List<T> objs, ToStr<T> label, ToStr<T> color, SetupCallback<T> setup)
        {
            layout.RemoveAllViews();
            foreach (T t in objs)
            {
                TextView view = GenerateTextView(label(t), color(t));
                setup?.Invoke(view, layout, t);
                layout.AddView(view);
            }
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

            // Add a 3dp margin below
            layoutParams.BottomMargin = DPToNearestPX(3f);

            // Add 5dp margins to the left/right
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