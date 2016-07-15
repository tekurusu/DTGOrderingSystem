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
        private static List<Order> orders;
        private Button addButton;
        private Button syncButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.orderList);

            mListView = FindViewById<ListView>(Resource.Id.orderListView);
            addButton = FindViewById<Button>(Resource.Id.orderAdd);
            syncButton = FindViewById<Button>(Resource.Id.syncButton);

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

            syncButton.Click += (object sender, EventArgs e) =>
            {
                DBRepository dbr = new DBRepository();
                dbr.deleteDB();

                dbr.createDB();

                string meat = dbr.insertCategory("Meat");
                string spices = dbr.insertCategory("Spices");
                string others = dbr.insertCategory("Others");

                dbr.insertItem("Chicken", "kilos", dbr.getCategory(meat));
                dbr.insertItem("Beef", "cows", dbr.getCategory(meat));
                dbr.insertItem("Pork", "pigs", dbr.getCategory(meat));
                dbr.insertItem("Paprika", "manyak", dbr.getCategory(spices));
                dbr.insertItem("Salt", "mats", dbr.getCategory(spices));
                dbr.insertItem("Sugar", "subjects", dbr.getCategory(spices));
                dbr.insertItem("Broom", "top 1", dbr.getCategory(others));
                dbr.insertItem("Fan", "grad school", dbr.getCategory(others));
                dbr.insertItem("Water", "lightning", dbr.getCategory(others));

                Toast.MakeText(this, "Initial database synced successfully", ToastLength.Short);
            };
        }
    }
}