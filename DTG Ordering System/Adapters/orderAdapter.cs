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
            if (orders[position].HasSent == true)
            {
                hasSent.Text = "Sent";
            }
			else
            {
                hasSent.Text = "Not Yet Sent";
            }
            Button deleteButton = row.FindViewById<Button>(Resource.Id.deleteButton);
            deleteButton.SetOnClickListener(new DeleteButtonClickListener(activity, orders[position]));
            Button editButton = row.FindViewById<Button>(Resource.Id.editButton);
            editButton.SetOnClickListener(new EditButtonClickListener(activity, orders[position]));
            //editButton.Click += (object sender, EventArgs e) => { Console.WriteLine("wew"); };




            return row;
        }
        private class EditButtonClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private Activity activity;
            private Order order;
            public EditButtonClickListener(Activity activity, Order order)
            {
                this.activity = activity;
                this.order = order;
            }

            public void OnClick(View v)
            {
                //string name = (string)v.Tag;
                //string text = string.Format("{0} Button Click.", name);
                //Toast.MakeText(this.activity, text, ToastLength.Short).Show();
                //Toast.MakeText(this.activity, order, ToastLength.Short).Show();
                Intent intent = new Intent(activity.ApplicationContext, typeof(NewOrderActivity));
                string json = JsonConvert.SerializeObject(order, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                intent.PutExtra("id", json);
                activity.StartActivity(intent);
            }
        }

        private class DeleteButtonClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private Activity activity;
            Order order = new Order();
            public DeleteButtonClickListener(Activity activity, Order order)
            {
                this.activity = activity;
                this.order = order;
            }

            public void OnClick(View v)
            {
                //string name = (string)v.Tag;
                //string text = string.Format("{0} Button Click.", name);
                Toast.MakeText(this.activity, order.Id, ToastLength.Short).Show();
                Console.WriteLine("grabe");
            }
        }
    }
}