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
	//[Activity(Label = "My Orders", MainLauncher = true, Icon = "@drawable/icon")]
	[Activity(Label = "My Orders",  Icon = "@drawable/icon")]
	public class OrdersActivity : Activity
    {
        private ListView mListView;
        private static List<Order> orders;
        private Button addButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.orderList);

            mListView = FindViewById<ListView>(Resource.Id.orderListView);
            addButton = FindViewById<Button>(Resource.Id.orderAdd);

            orders = new List<Order>();
            orders.Add(new Order() { Id = 1000, DeliveryDate = "6/02/16", HasSent = true });
            orders.Add(new Order() { Id = 1001, DeliveryDate = "6/03/16", HasSent = false });
            orders.Add(new Order() { Id = 1002, DeliveryDate = "6/17/16", HasSent = true });

            OrderAdapter adapter = new OrderAdapter(this, orders);
            mListView.Adapter = adapter;
            addButton.Click += delegate
            {
                Intent intent = new Intent(this.ApplicationContext, typeof(NewOrderActivity));
                StartActivity(intent);
            };
        }
    }
}