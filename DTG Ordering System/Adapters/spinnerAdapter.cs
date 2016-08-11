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

namespace DTG_Ordering_System
{
    class SpinnerAdapter : BaseAdapter<string>
    {
        private List<String> branches;
        private Context context;
        private Activity activity;

        public SpinnerAdapter(Activity activity, List<String> branches, Context context)
        {
            this.context = context;
            this.branches = branches;
            this.activity = activity;
        }

        public override int Count
        {
            get { return branches.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override String this[int position]
        {
            get { return branches[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(context).Inflate(Resource.Layout.spinnerCustomRow, null, false);
            }

            TextView branch = row.FindViewById<TextView>(Resource.Id.spinnerEntry);

            if (position == 0)
            {
                branch.Text = "";
                branch.Hint = this[0];
            }
            else
            {
                branch.Text = branches[position];
            }

            return row;
        }


    }
}