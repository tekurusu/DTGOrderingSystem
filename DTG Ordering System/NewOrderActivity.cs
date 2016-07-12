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
    [Activity(Label = "New Order", Icon = "@drawable/icon")]
    public class NewOrderActivity : Activity
    {
        private static List<Item> items = new List<Item>();
        private ListView mListView;
        private newOrderAdapter adapter;
        private Button addNewButton;
        private TextView deliveryDate;
        private Button editDate;
        private DateTime dateHolder;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.newOrder);

            mListView = FindViewById<ListView>(Resource.Id.selectedItemsListView);
            addNewButton = FindViewById<Button>(Resource.Id.addNewItem);
            deliveryDate = FindViewById<TextView>(Resource.Id.deliveryDate);
            editDate = FindViewById<Button>(Resource.Id.editDate);

            DateTime now = DateTime.Now.ToLocalTime();
            dateHolder = now;
            String dateNow = String.Format("{0:dd MMM yy}", now);
            deliveryDate.Text = dateNow;
            editDate.Click += (object sender, EventArgs e) =>
            {
                DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    //deliveryDate.Text = time.ToLongDateString();
                    deliveryDate.Text = String.Format("{0:dd MMM yy}", time);
                    dateHolder = time;
                });
                Bundle args = new Bundle();
                args.PutInt("year", dateHolder.Year);
                args.PutInt("month", dateHolder.Month);
                args.PutInt("day", dateHolder.Day);

                frag.Arguments = args;
                frag.Show(FragmentManager, DatePickerFragment.TAG);
            };

            items.Clear();

                items.Add(new Item() { Name = "Chicken", Unit = "cuts", Quantity = 50 });
                items.Add(new Item() { Name = "Pork", Unit = "pigs", Quantity = 32 });
                items.Add(new Item() { Name = "Fish", Unit = "fillets", Quantity = 5 });

            adapter = new newOrderAdapter(this, items);

            Item indexerTest = adapter[1]; //Item at index 1

            mListView.Adapter = adapter;

            addNewButton.Click += (object sender, EventArgs e) =>
            {
                Intent intent = new Intent(this.ApplicationContext, typeof(CategoriesActivity));
                StartActivity(intent);
            };
            
        }
    }
}