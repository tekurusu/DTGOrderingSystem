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
    class OrderAdapter : BaseAdapter<Order>
    {
        private List<Order> orders;
        private Context context;
        private Activity activity;
        public OrderAdapter(Activity activity, List<Order> orders, Context context)
        {
            this.context = context;
            this.orders = orders;
            this.activity = activity;
        }

        public override int Count
        {
            get { return orders.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Order this[int position]
        {
            get { return orders[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(context).Inflate(Resource.Layout.orderListCustomRow, null, false);
            }

            TextView orderDate = row.FindViewById<TextView>(Resource.Id.orderDate);
            orderDate.Text = orders[position].DeliveryDate;

            TextView hasSent = row.FindViewById<TextView>(Resource.Id.hasSent);

            hasSent.Text = (orders[position].HasSent) ? "Sent" : "Draft";

            return row;
        }
    }
}