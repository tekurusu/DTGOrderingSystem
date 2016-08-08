using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace DTG_Ordering_System
{
	[Activity(Label = "My Orders", Icon = "@drawable/icon")]
	public class OrdersActivity : Activity
    {
        private ListView mListView;
        private static List<Order> orders = new List<Order>();
        private Button addButton;
        private Button syncButton;
        private OrderAdapter adapter;
        DBRepository dbr = new DBRepository();
        private string branchId;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            SetContentView(Resource.Layout.orderList);
            branchId = Intent.GetStringExtra("branchId");
            orders = dbr.getAllOrders(branchId);
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
                    intent.PutExtra("branchId", branchId);
                    StartActivityForResult(intent, 0);
                };
            }

			mListView.ItemLongClick += DeleteOrder_OnLongClick;

			addButton.Click += delegate
            {
                //Toast.MakeText(this, branchId.ToString(), ToastLength.Long).Show();
                Intent intent = new Intent(this.ApplicationContext, typeof(NewOrderActivity));
                intent.PutExtra("branchId", branchId);
                StartActivity(intent);
            };

			syncButton.Click += SyncButton_OnClick;
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            try
            {

                var orderId = data.GetStringExtra("OrderId");
                orders.Add(dbr.getOrder(orderId));
                adapter.NotifyDataSetChanged();
                


            }
            catch { }
        }

		void SyncButton_OnClick(object sender, EventArgs e)
		{
            Toast.MakeText(this, branchId.ToString(), ToastLength.Long).Show();
			//var progressDialog = ProgressDialog.Show(this, "Please wait...", "Syncing Database...", true);
			//new Thread(new ThreadStart(delegate
			//{
			//	dbr.syncDB();
			//	//hide progress dialogue
			//	RunOnUiThread(() => progressDialog.Hide());
			//})).Start();

			//orders.Clear();
			//adapter.NotifyDataSetChanged();
		}

		void DeleteOrder_OnLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
		{
			if (orders[e.Position].HasSent == false)
			{
                //Toast.MakeText(this, orders[e.Position].BranchId.ToString(), ToastLength.Long).Show();
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
					Intent intent = new Intent(this, typeof(NewOrderActivity));
					intent.PutExtra("orderId", orders[e.Position].Id);
					intent.PutExtra("hasSent", orders[e.Position].HasSent);
					intent.PutExtra("replacement", true);
					StartActivityForResult(intent, 0);
				});
				callDialog.SetNegativeButton("No", delegate { });
				callDialog.Show();
			}
		}

        public override void OnBackPressed()
        {
            var callDialog = new AlertDialog.Builder(this);
            callDialog.SetMessage("Close DTG Ordering System app?");
            callDialog.SetNeutralButton("Yes", delegate
            {
                this.FinishAffinity();
            });
            callDialog.SetNegativeButton("No", delegate { });
            callDialog.Show();
        }
    }
}