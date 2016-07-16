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
    class OrderAdapter : BaseAdapter<Order>
    {
        private List<Order> orders;
        private Context context;

        public OrderAdapter(Context context, List<Order> orders)
        {
            this.context = context;
            this.orders = orders;
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

            TextView orderNumber = row.FindViewById<TextView>(Resource.Id.orderNumber);
            orderNumber.Text = orders[position].Id.ToString();

            TextView orderDate = row.FindViewById<TextView>(Resource.Id.orderDate);
            orderDate.Text = orders[position].DeliveryDate;

            TextView hasSent = row.FindViewById<TextView>(Resource.Id.hasSent);

            Button editButton = row.FindViewById<Button>(Resource.Id.orderEdit);
            editButton.Click += (object sender, EventArgs e) => { Console.WriteLine("wew"); };
            Button deleteButton = row.FindViewById<Button>(Resource.Id.orderDelete);
            deleteButton.Click += (object sender, EventArgs e) => { Console.WriteLine("weh"); };

            return row;
        }
    }
}