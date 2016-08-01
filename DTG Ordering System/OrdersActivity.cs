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
	[Activity(Label = "My Orders", MainLauncher = true, Icon = "@drawable/icon")]
	public class OrdersActivity : Activity
    {
        private ListView mListView;
        private static List<Order> orders = new List<Order>();
        private Button addButton;
        private Button syncButton;
        private OrderAdapter adapter;
        DBRepository dbr = new DBRepository();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.orderList);
            
            orders = dbr.getAllOrders();
			mListView = FindViewById<ListView>(Resource.Id.orderListView);
            mListView.Clickable = true;
            addButton = FindViewById<Button>(Resource.Id.orderAdd);
            syncButton = FindViewById<Button>(Resource.Id.syncButton);
            
            adapter = new OrderAdapter(this, orders, this);
            mListView.Adapter = adapter;
            
            if (mListView != null)
            {
                mListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
                {
                    Intent intent = new Intent(this, typeof(NewOrderActivity));
                    intent.PutExtra("orderId", orders[e.Position].Id);
					intent.PutExtra("hasSent", orders[e.Position].HasSent);
                    StartActivityForResult(intent, 0);
                };
            }

			mListView.ItemLongClick += DeleteOrder_OnLongClick;

			addButton.Click += delegate
            {
                Intent intent = new Intent(this.ApplicationContext, typeof(NewOrderActivity));
                StartActivity(intent);
            };

            syncButton.Click += (object sender, EventArgs e) =>
            { 
                dbr.syncDB();

                Toast.MakeText(this, "Database reloaded from scratch!", ToastLength.Short).Show();

                orders.Clear();
                adapter.NotifyDataSetChanged();
            };
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            try
            {
                var orderId = data.GetStringExtra("orderId");
                orders.Add(dbr.getOrder(orderId));

                adapter.NotifyDataSetChanged();
            }
            catch { }
        }

		void DeleteOrder_OnLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
		{
			if (orders[e.Position].HasSent == false)
			{
				var callDialog = new AlertDialog.Builder(this);
				callDialog.SetMessage("Delete " + orders[e.Position].DeliveryDate + "?");
				callDialog.SetNeutralButton("Delete", delegate
				{
					DBRepository dbr = new DBRepository();
					dbr.deleteOrder(orders[e.Position].Id);
					orders.RemoveAt(e.Position);
					adapter.NotifyDataSetChanged();
				});
				callDialog.SetNegativeButton("Cancel", delegate { });
				callDialog.Show();
			}
			else
			{ 
				var callDialog = new AlertDialog.Builder(this);
				callDialog.SetMessage("Edit order: " + orders[e.Position].DeliveryDate + " for replacement?");
				callDialog.SetNeutralButton("Yes", delegate
				{
					//insert replace order function here
				});
				callDialog.SetNegativeButton("No", delegate { });
				callDialog.Show();
			}
		}
    }
}