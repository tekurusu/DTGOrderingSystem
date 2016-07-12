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
        private ListView mListView;
        private int currentPosition;
        private ItemAdapter adapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.itemList);
            mListView = FindViewById<ListView>(Resource.Id.itemListView);

            //get categoryID from previous activity
            var category = Intent.Extras.GetInt("CategoryID");
            items.Clear();

            if (category == 0)  //Meat
            {
                items.Add(new Item() { Name = "Chicken", Unit = "cuts" });
                items.Add(new Item() { Name = "Pork", Unit = "pigs" });
                items.Add(new Item() { Name = "Fish", Unit = "fillets" });
            }

            else if (category == 1) //Spices
            {
                items.Add(new Item() { Name = "Salt", Unit = "grams" });
                items.Add(new Item() { Name = "Sugar", Unit = "grams" });
                items.Add(new Item() { Name = "Pepper", Unit = "grams" });
                items.Add(new Item() { Name = "Paprica", Unit = "kilos" });
                items.Add(new Item() { Name = "Basil", Unit = "kilos" });
                items.Add(new Item() { Name = "Cinnamon", Unit = "kilos" });
                items.Add(new Item() { Name = "Chili", Unit = "kilos" });
            }

            else if (category == 2) //Others
            {
                items.Add(new Item() { Name = "Wilbert Uy", Unit = "UPE" });
                items.Add(new Item() { Name = "Jerome Tec", Unit = "98" });
                items.Add(new Item() { Name = "Zarah Arcega", Unit = "1910" });
            }

            adapter = new ItemAdapter(this, items);

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
            //var editer = this.mListView.GetChildAt(currentPosition);
            //editer.FindViewById<TextView>(Resource.Id.itemQuantity).Text = e.Quantity.ToString();
            items[e.Position].Quantity = e.Quantity;
            adapter.NotifyDataSetChanged();
        }
    }
}

