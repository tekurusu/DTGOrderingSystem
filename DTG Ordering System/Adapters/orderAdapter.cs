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
			hasSent.Text = orders[position].HasSent.ToString();
            Button deleteButton = row.FindViewById<Button>(Resource.Id.deleteButton);
            deleteButton.SetOnClickListener(new DeleteButtonClickListener(activity));
            Button editButton = row.FindViewById<Button>(Resource.Id.editButton);
            editButton.SetOnClickListener(new EditButtonClickListener(activity));
            //editButton.Click += (object sender, EventArgs e) => { Console.WriteLine("wew"); };




            return row;
        }
        private class EditButtonClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private Activity activity;

            public EditButtonClickListener(Activity activity)
            {
                this.activity = activity;
            }

            public void OnClick(View v)
            {
                //string name = (string)v.Tag;
                //string text = string.Format("{0} Button Click.", name);
                //Toast.MakeText(this.activity, text, ToastLength.Short).Show();
                Console.WriteLine("wew");
                Intent intent = new Intent(activity.ApplicationContext, typeof(CategoriesActivity));
                activity.StartActivity(intent);
            }
        }

        private class DeleteButtonClickListener : Java.Lang.Object, View.IOnClickListener
        {
            private Activity activity;

            public DeleteButtonClickListener(Activity activity)
            {
                this.activity = activity;
            }

            public void OnClick(View v)
            {
                //string name = (string)v.Tag;
                //string text = string.Format("{0} Button Click.", name);
                //Toast.MakeText(this.activity, text, ToastLength.Short).Show();
                Console.WriteLine("grabe");
            }
        }
    }
}