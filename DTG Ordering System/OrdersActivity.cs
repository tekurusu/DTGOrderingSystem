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

            OrderAdapter adapter = new OrderAdapter(this, orders, this);
            mListView.Adapter = adapter;
            if (mListView != null)
            {
                mListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
                {

                    var intent = new Intent(this, typeof(NewOrderActivity));
                    intent.PutExtra("Id", orders[e.Position].Id);
                    StartActivity(intent);
                    //OrderFragment fragment = new OrderFragment();
                    //Bundle args = new Bundle();
                    //args.PutInt("Index", orders[e.Position].Id);
                    //fragment.Show(this.FragmentManager, "Hello");


                };
            }
            addButton.Click += delegate
            {
                Intent intent = new Intent(this.ApplicationContext, typeof(NewOrderActivity));
                StartActivity(intent);
            };

            syncButton.Click += (object sender, EventArgs e) =>
            {
                DBRepository dbr = new DBRepository();
                dbr.syncDB();

                Toast.MakeText(this, "Database reloaded from scratch!", ToastLength.Short).Show();
            };
        }

        //public void myClickHandler(View v)
        //{
        //    mListView = FindViewById<ListView>(Resource.Id.orderListView);
        //    for (int i=0; i<mListView.ChildCount; i++)
        //    {
        //        mListView.GetChildAt(i).SetBackgroundColor(Android.Graphics.Color.Blue);
        //    }
        //    LinearLayout parent = (LinearLayout)v.Parent;
        //    TextView child = (TextView)parent.GetChildAt(0);
        //    Button btnChild = (Button)parent.GetChildAt(1);

        //    int c = Android.Graphics.Color.Cyan;

        //}
    }
}