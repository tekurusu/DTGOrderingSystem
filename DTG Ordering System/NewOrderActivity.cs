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
    [Activity(Label = "New Order", Icon = "@drawable/icon")]
    public class NewOrderActivity : Activity
    {
        private static List<Item> items = new List<Item>();
        private static List<OrderedItem> orderedItems = new List<OrderedItem>();
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

            //code for datepicker
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

            try
            {
                adapter = new newOrderAdapter(this, items);
                mListView.Adapter = adapter;
            } catch
            {

            }

            addNewButton.Click += (object sender, EventArgs e) =>
            {
                Intent intent = new Intent(this.ApplicationContext, typeof(CategoriesActivity));
                StartActivityForResult(intent,0);
            };

			mListView.ItemLongClick += DeleteItem_OnLongClick;
		}

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            //if the activity is ok retrieve json then add to the item list.
            if (resultCode == Result.Ok)
            {
                var message = data.GetStringExtra("addedItems");
                List<Item> addedItems = JsonConvert.DeserializeObject<List<Item>>(message);

                foreach (Item i in addedItems)
                {
                    if (items.Exists(item => item.Id == i.Id) == true)
                    {
                        items.Find(item => item.Id == i.Id).Quantity += i.Quantity;
                    }

                    else
                    {
                        items.Add(i);
                    }
                }

                adapter.NotifyDataSetChanged();
            }
        }

		void DeleteItem_OnLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
		{
			var callDialog = new AlertDialog.Builder(this);
			callDialog.SetMessage("Delete " + items[e.Position].Name + "?");
			callDialog.SetNeutralButton("Delete", delegate
			{
				items.RemoveAt(e.Position);
				adapter.NotifyDataSetChanged();

			});
			callDialog.SetNegativeButton("Cancel", delegate { });
			callDialog.Show();
		}
    }
}