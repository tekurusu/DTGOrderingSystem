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
	[Activity(Label = "My Orders", MainLauncher = true, Icon = "@drawable/icon")]
	public class OrdersActivity : Activity
    {
        private ListView mListView;
        private static List<Order> orders = new List<Order>();
        private Button addButton;
        private Button syncButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.orderList);

            mListView = FindViewById<ListView>(Resource.Id.orderListView);
            addButton = FindViewById<Button>(Resource.Id.orderAdd);
            syncButton = FindViewById<Button>(Resource.Id.syncButton);

            OrderAdapter adapter = new OrderAdapter(this, orders);
            mListView.Adapter = adapter;
            addButton.Click += delegate
            {
                Intent intent = new Intent(this.ApplicationContext, typeof(NewOrderActivity));
                StartActivity(intent);
            };

            syncButton.Click += (object sender, EventArgs e) =>
            {
                DBRepository dbr = new DBRepository();
                dbr.syncDB();

                Toast.MakeText(this, "Initial database synced successfully", ToastLength.Short).Show();
            };
        }
    }
}