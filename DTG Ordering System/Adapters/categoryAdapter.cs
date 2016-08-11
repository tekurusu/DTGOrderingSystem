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
using Newtonsoft.Json;

namespace DTG_Ordering_System
{
    class CategoryAdapter : BaseAdapter<String>
    {
        private List<String> categories;
        private Context context;
        private Activity activity;
        public CategoryAdapter(Activity activity, List<String> categories, Context context)
        {
            this.context = context;
            this.categories = categories;
            this.activity = activity;
        }

        public override int Count
        {
            get { return categories.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override String this[int position]
        {
            get { return categories[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(context).Inflate(Resource.Layout.categoryCustomRow, null, false);
            }

            TextView categoryName = row.FindViewById<TextView>(Resource.Id.categoryRow);
            categoryName.Text = categories[position];

            return row;
        }
    }
}