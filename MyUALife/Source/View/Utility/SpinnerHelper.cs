using System;
using System.Collections.Generic;
using Android.Widget;

namespace MyUALife
{
    class SpinnerHelper<T>
    {
        // The spinner this SpinnerHelper supports
        public Spinner Spinner
        {
            get;
            private set;
        }

        /*
         * The item currently selected by the spinner
         */
        public T SelectedItem
        {
            get
            {
                String name = Spinner.SelectedItem.ToString();
                foreach (T t in items)
                {
                    if (nameRule(t) == name)
                    {
                        return t;
                    }
                }
                return default(T);
            }

            set
            {
                int index = adapter.GetPosition(nameRule(value));
                Spinner.SetSelection(index);
            }
        }

        // The rule for converting from objects to names
        private readonly ToStr<T> nameRule;

        // The adapter for the spinner
        private readonly ArrayAdapter<String> adapter;

        // The list of items displayed by the spinner
        private readonly List<T> items;

        /*
         * Creates a SpinnerHelper that initializes the supplied spinner to
         * contain the names of the Ts in items. The names of those items are
         * determined using nameRule.
         */
        public SpinnerHelper(Spinner spinner, List<T> items, ToStr<T> nameRule)
        {
            Spinner = spinner;
            this.nameRule = nameRule;
            this.items = items;

            List<String> names = new List<String>();
            foreach (T item in items)
            {
                names.Add(nameRule(item));
            }
            adapter = new ArrayAdapter<String>(spinner.Context, Android.Resource.Layout.SimpleSpinnerDropDownItem, names);
            spinner.Adapter = adapter;
        }
    }
}