using Android.Content;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Views;
using System;
using System.Collections.Generic;

namespace MyUALife
{
    // Function for converting a generic object into a string
    public delegate String ToStr<T>(T t);

    public static class ViewUtil
    {
        // Function to perform setup operations on a textView
        public delegate void SetupCallback<T>(TextView textView, LinearLayout layout, T target);

        /*
         * Fills the supplied layout with TextViews representing the objects in
         * objs. Views are added in the order that objects appear in the list.
         * The text displayed in the TextView is given by label(obj). The
         * background of the TextView is a rounded rectangle with the color
         * represented by color(obj). Each TextView is setup with setup before
         * it is added to the layout.
         */
        public static void LoadListToLayout<T>(LinearLayout layout, List<T> objs, ToStr<T> label, ToStr<T> color, SetupCallback<T> setup)
        {
            layout.RemoveAllViews();
            foreach (T t in objs)
            {
                TextView view = GenerateTextView(layout.Context, label(t), color(t));
                setup?.Invoke(view, layout, t);
                layout.AddView(view);
            }
        }

        /*
         * Extends ContextWrapper to create a TextView that contains the
         * specified text. The background of the TextView will be a rounded
         * rectangle of the specified color.
         */
        public static TextView GenerateTextView(Context context, String text, String colorString)
        {
            // Create the text view
            TextView view = new TextView(context);

            // Add 6dp of padding on the left and right
            int padding = context.DPToNearestPX(6f);
            view.SetPadding(view.PaddingLeft + padding, view.PaddingTop, view.PaddingRight + padding, view.PaddingBottom);

            // Create a LayoutParams to set the margins
            var layoutParams = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            view.LayoutParameters = layoutParams;

            // Add a 3dp margin below
            layoutParams.BottomMargin = context.DPToNearestPX(3f);

            // Add 5dp margins to the left/right
            int margin = context.DPToNearestPX(5f);
            layoutParams.LeftMargin = margin;
            layoutParams.RightMargin = margin;

            // Set the background to be a rounded rectangle
            float radius = context.DPToPX(4f);
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
         * Extends Context to convert a given length in dp to px.
         */
        public static float DPToPX(this Context context, float dp)
        {
            float density = context.Resources.DisplayMetrics.Density;
            return density * dp;
        }

        /*
         * Extends Context to convert a given length in dp to px. Rounds to the
         * nearest int.
         */
        public static int DPToNearestPX(this Context context, float dp)
        {
            float density = context.Resources.DisplayMetrics.Density;
            return (int)(dp * density + 0.5f);
        }
    }
}