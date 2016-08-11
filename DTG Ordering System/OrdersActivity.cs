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
using Android.Preferences;
using System.ServiceModel;

namespace DTG_Ordering_System
{
	[Activity(Icon = "@drawable/icon")]
	public class OrdersActivity : Activity
    {
        private ListView mListView;
        private static List<Order> orders = new List<Order>();
        private Button addButton;
        private Button syncButton;
        private ImageButton backButton;
        private OrderAdapter adapter;
        DBRepository dbr = new DBRepository();
        private string branchId;
        private Service1Client _client;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            SetContentView(Resource.Layout.orderList);

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            //branchId = Intent.GetStringExtra("branchId");
            branchId = prefs.GetString("branchId", null);

            this.Title = dbr.getBranchName(branchId) + " Orders";

            if ((Intent.HasExtra("OrderId") == true) && (Intent.HasExtra("isFromEdit") == false))
            {
                Order o = dbr.getOrder(Intent.GetStringExtra("OrderId"));
                dbr.updateOrderStatus(o.Id, true);
            }

            orders = dbr.getAllOrders(branchId);
            //Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            TextView branchName = FindViewById<TextView>(Resource.Id.branchName);
            TextView logout = FindViewById<TextView>(Resource.Id.logout);
			mListView = FindViewById<ListView>(Resource.Id.orderListView);
            mListView.Clickable = true;
            addButton = FindViewById<Button>(Resource.Id.orderAdd);
            syncButton = FindViewById<Button>(Resource.Id.syncButton);
            backButton = FindViewById<ImageButton>(Resource.Id.backButton);
            //SetActionBar(toolbar);
            //ActionBar.SetDisplayShowTitleEnabled(false);
            branchName.Text = dbr.getBranchName(branchId);
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
                //Toast.MakeText(this, branchId.ToString(), ToastLength.Long).Show();
                Intent intent = new Intent(this.ApplicationContext, typeof(NewOrderActivity));
                StartActivity(intent);
            };
            logout.SetOnClickListener(new LogoutClickListener(this));
            backButton.Click += BackButton_Click;
        }

        //private class LogoutClickListener : Java.Lang.Object, View.IOnClickListener
        //{
        //    private Activity activity;
        //    public LogoutClickListener(Activity activity)
        //    {
        //        this.activity = activity;
        //    }
        //    public void OnClick(View v)
        //    {
        //        var callDialog = new AlertDialog.Builder(activity);
        //        callDialog.SetMessage("Are you sure you want to logout?");
        //        callDialog.SetNeutralButton("Yes", delegate
        //        {
        //            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(activity);
        //            ISharedPreferencesEditor editor = prefs.Edit();
        //            editor.Clear();
        //            editor.Apply();

        //            Intent intent = new Intent(activity.ApplicationContext, typeof(LoginActivity));
        //            activity.StartActivityForResult(intent, 1);
        //        });
        //        callDialog.SetNegativeButton("No", delegate { });
        //        callDialog.Show();
        //    }
        //}

        void LogoutButton_OnClick(object sender, EventArgs e)
        {
            var callDialog = new AlertDialog.Builder(this);
            callDialog.SetMessage("Are you sure you want to logout?");
            callDialog.SetNeutralButton("Yes", delegate
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.Clear();
                editor.Apply();

                Intent intent = new Intent(ApplicationContext, typeof(LoginActivity));
                StartActivityForResult(intent, 1);
            });
            callDialog.SetNegativeButton("No", delegate { });
            callDialog.Show();
            //Toast.MakeText(this, branchId.ToString(), ToastLength.Long).Show();
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

        private void BackButton_Click(object sender, EventArgs e)
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
            var callDialog = new AlertDialog.Builder(this);
            callDialog.SetMessage("Are you sure you want to logout?");
            callDialog.SetNeutralButton("Yes", delegate
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.Clear();
                editor.Apply();

                Intent intent = new Intent(ApplicationContext, typeof(LoginActivity));
                StartActivityForResult(intent, 1);
            });
            callDialog.SetNegativeButton("No", delegate { });
            callDialog.Show();
            //Toast.MakeText(this, branchId.ToString(), ToastLength.Long).Show();

            //var progressDialog = ProgressDialog.Show(this, "Please wait...", "Syncing Database...", true);
            //new Thread(new ThreadStart(delegate
            //{
            //    dbr.syncDB();
            //    //hide progress dialogue
            //    RunOnUiThread(() => progressDialog.Hide());
            //})).Start();

            //orders.Clear();
            //adapter.NotifyDataSetChanged();
        }

        //private void InitializeService1Client()
        //{
        //    BasicHttpBinding binding = CreateBasicHttp();

        //    _client = new Service1Client(binding, dbr.getIP());
        //    _client.getAllItemsCompleted += _client_getAllItemsCompleted;
        //    _client.getAllCategoriesCompleted += _client_getAllICategoriesCompleted;
        //}

        //private void _client_getAllICategoriesCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //{
        //    string msg = null;

        //    if (e.Error != null)
        //    {
        //        msg = e.Error.Message;
        //    }
        //    else if (e.Cancelled)
        //    {
        //        msg = "Request was cancelled.";
        //    }
        //    else
        //    {
        //        msg = "All categories synced.";
        //    }
        //    RunOnUiThread(() => Toast.MakeText(this, msg, ToastLength.Long).Show());
        //}

        //private void _client_getAllItemsCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //{
        //    string msg = null;

        //    if (e.Error != null)
        //    {
        //        msg = e.Error.Message;
        //    }
        //    else if (e.Cancelled)
        //    {
        //        msg = "Request was cancelled.";
        //    }
        //    else
        //    {
        //        msg = "All items synced.";
        //    }
        //    RunOnUiThread(() => Toast.MakeText(this, msg, ToastLength.Long).Show());
        //}

        //private static BasicHttpBinding CreateBasicHttp()
        //{
        //    BasicHttpBinding binding = new BasicHttpBinding
        //    {
        //        Name = "basicHttpBinding",
        //        MaxBufferSize = 2147483647,
        //        MaxReceivedMessageSize = 2147483647
        //    };
        //    TimeSpan timeout = new TimeSpan(0, 0, 30);
        //    binding.SendTimeout = timeout;
        //    binding.OpenTimeout = timeout;
        //    binding.ReceiveTimeout = timeout;
        //    return binding;
        //}

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