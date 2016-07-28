using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DTG_Ordering_System
{
    [Activity(Label = "Item Screen", Icon = "@drawable/icon")]
    public class ItemsActivity : Activity
    {
        private static List<Item> items = new List<Item>();
        private static List<Item> addedItems = new List<Item>();
        private static List<Category> categories = new List<Category>();
        private ListView mListView;
        private ItemAdapter adapter;
        private Button itemAdd;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.itemList);
            this.Title = Intent.Extras.GetString("categoryName");

            mListView = FindViewById<ListView>(Resource.Id.itemListView);
            itemAdd = FindViewById<Button>(Resource.Id.itemAdd);

            //clear lists if start of new activity.
			items.Clear();
            addedItems.Clear();

			DBRepository dbr = new DBRepository();
            dbr.clearQuantity(Intent.Extras.GetString("categoryId"));
            items = dbr.getAllItems(Intent.Extras.GetString("categoryId"));

            adapter = new ItemAdapter(this, items);

            //testing the adapter
            //Item indexerTest = adapter[1]; //Item at index 1

            mListView.Adapter = adapter;
            mListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) =>
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                NumberPickerFragment picker = new NumberPickerFragment();

                Bundle args = new Bundle();
                args.PutInt("quantity", items[e.Position].Quantity);
				args.PutInt("position", e.Position);
                args.PutString("itemName", items[e.Position].Name);

				picker.Arguments = args;
                picker.Show(transaction, "dialog fragment");

                picker.onNumberPickComplete += Picker_onNumberPickComplete;
            };

            itemAdd.Click += delegate
            {
                Intent intent = new Intent();
                string json = JsonConvert.SerializeObject(addedItems, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                intent.PutExtra("addedItems", json);
                SetResult(Result.Ok, intent);
                Finish();
            };
        }

        private void Picker_onNumberPickComplete(object sender, OnNumberPickEventArgs e)
        {
			DBRepository dbr = new DBRepository();

			dbr.setQuantity(items[e.Position].Id, e.Quantity);
            if (e.Quantity != 0)
            {
                addedItems.Add(items[e.Position]);
            }

            adapter.NotifyDataSetChanged();
        }

        public override void OnBackPressed()
        {
            if (addedItems.Count > 0)
            {
                var callDialog = new AlertDialog.Builder(this);
                callDialog.SetMessage("Are you sure you want to discard the quantities specified in this category?");
                callDialog.SetNeutralButton("Yes", delegate
                {
                    base.OnBackPressed();
                });
                callDialog.SetNegativeButton("No", delegate { });
                callDialog.Show();
            }
            else
            {
                base.OnBackPressed();
            }    
        }
    }
}

