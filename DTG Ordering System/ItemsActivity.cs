using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;

namespace DTG_Ordering_System
{
    [Activity(Label = "Item Screen", Icon = "@drawable/icon")]
    public class ItemsActivity : Activity
    {
        private static List<Item> items = new List<Item>();
		private static List<Category> categories = new List<Category>();
		private static List<String> categoryId = new List<String>();
        private ListView mListView;
        private int currentPosition;
        private ItemAdapter adapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.itemList);
            mListView = FindViewById<ListView>(Resource.Id.itemListView);

			var catID = Intent.Extras.GetString("CategoryID");
			items.Clear();

			DBRepository dbr = new DBRepository();

			categories = dbr.getAllCategories();

			foreach (Category c in categories)
			{
				if (catID == c.Id)
				{
					items = dbr.getAllItems(catID);
				}
			}

            adapter = new ItemAdapter(this, items);

            //testing the adapter
            Item indexerTest = adapter[1]; //Item at index 1

            mListView.Adapter = adapter;
            mListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                currentPosition = e.Position;
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                NumberPickerFragment picker = new NumberPickerFragment();

                Bundle args = new Bundle();
                args.PutInt("quantity", items[e.Position].Quantity);
				args.PutInt("position", e.Position);

				picker.Arguments = args;
                picker.Show(transaction, "dialog fragment");

                picker.onNumberPickComplete += Picker_onNumberPickComplete;
            };
        }

        private void Picker_onNumberPickComplete(object sender, OnNumberPickEventArgs e)
        {
			DBRepository dbr = new DBRepository();

			dbr.setQuantity(items[e.Position].Id, e.Quantity);

            adapter.NotifyDataSetChanged();
        }
    }
}

