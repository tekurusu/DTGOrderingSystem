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
    class newOrderAdapter : BaseAdapter<Item>
    {
        private List<Item> items;
        private Context context;

        public newOrderAdapter(Context context, List<Item> items)
        {
            this.context = context;
            this.items = items;
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Item this[int position]
        {
            get { return items[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(context).Inflate(Resource.Layout.newOrderCustomRow, null, false);
            }

            TextView itemName = row.FindViewById<TextView>(Resource.Id.itemName);
            itemName.Text = items[position].Name;

            TextView itemUnit = row.FindViewById<TextView>(Resource.Id.itemUnit);
            itemUnit.Text = items[position].Unit;

            TextView itemQuantity = row.FindViewById<TextView>(Resource.Id.itemQuantity);
            itemQuantity.Text = items[position].Quantity.ToString();

            return row;
        }
    }
}