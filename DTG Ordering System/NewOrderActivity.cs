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
		private TextView deliveryDate;
        private Button addItemsButton;
		private Button saveButton;
		private Button sendButton;
        private Button editDate;
        private DateTime dateHolder;
        private bool changeIsComing;
        DBRepository dbr = new DBRepository();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.newOrder);

            mListView = FindViewById<ListView>(Resource.Id.selectedItemsListView);
            addItemsButton = FindViewById<Button>(Resource.Id.addItems);
            saveButton = FindViewById<Button>(Resource.Id.saveButton);
            sendButton = FindViewById<Button>(Resource.Id.sendButton);
            deliveryDate = FindViewById<TextView>(Resource.Id.deliveryDate);
            editDate = FindViewById<Button>(Resource.Id.editDate);
            changeIsComing = false;
                
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

            items.Clear();
            adapter = new newOrderAdapter(this, items);
            mListView.Adapter = adapter;

            addItemsButton.Click += (object sender, EventArgs e) =>
            {
                Intent intent = new Intent(this.ApplicationContext, typeof(CategoriesActivity));
                StartActivityForResult(intent, 0);
            };

            mListView.ItemLongClick += DeleteItem_OnLongClick;
            saveButton.Click += SaveButton_OnClick;
            sendButton.Click += SendButton_OnClick;

            if (Intent.GetStringExtra("orderId") != null)
            {
                Order order = dbr.getOrder(Intent.GetStringExtra("orderId"));
                List<OrderedItem> orderedItems = dbr.getAllOrderedItems(order.Id);

                deliveryDate.Text = String.Format("{0:dd MMM yy}", DateTime.Parse(order.DeliveryDate));
                foreach (OrderedItem oi in orderedItems)
                {
                    items.Add(oi.Item);
                }

                adapter.NotifyDataSetChanged();
            }
        }

		void SaveButton_OnClick(object sender, EventArgs e)
		{
			if (items.Count == 0)
			{
				Toast.MakeText(this, "There are no items to be saved.", ToastLength.Long).Show();
			}
			else
			{
				var callDialog = new AlertDialog.Builder(this);
				callDialog.SetMessage("Are you sure you want to save this order as a draft?");
				callDialog.SetNeutralButton("OK", delegate
				{
					DBRepository dbr = new DBRepository();
                    string orderId;

                    if (Intent.GetStringExtra("orderId") == null)
                    {
                        orderId = dbr.insertOrder(deliveryDate.Text);
                        dbr.insertOrderedItems(items, orderId);
                    }
                    else
                    {
                        orderId = Intent.GetStringExtra("orderId");
                        dbr.updateOrder(orderId, deliveryDate.Text);
                        dbr.updateOrderedItems(items, orderId);
                    }

                    Intent intent = new Intent(ApplicationContext, typeof(OrdersActivity));
					intent.PutExtra("OrderId", orderId);
					StartActivityForResult(intent, 1);

					items.Clear();
					adapter.NotifyDataSetChanged();
				});
				callDialog.SetNegativeButton("Cancel", delegate { });
				callDialog.Show();
			}
			
		}

		void SendButton_OnClick(object sender, EventArgs e)
		{
			if (items.Count == 0)
			{
				Toast.MakeText(this, "There are no items to be sent.", ToastLength.Long).Show();
			}
			else
			{
				var callDialog = new AlertDialog.Builder(this);
				callDialog.SetMessage("Are you sure you want to send this order?");
				callDialog.SetNeutralButton("OK", delegate
				{
					DBRepository dbr = new DBRepository();
					string orderId = dbr.insertOrder(deliveryDate.Text);
					dbr.insertOrderedItems(items, orderId);
					dbr.sendOrder(orderId);

					Intent intent = new Intent(ApplicationContext, typeof(OrdersActivity));
					intent.PutExtra("OrderId", orderId);
                    StartActivityForResult(intent, 1);

					items.Clear();
					adapter.NotifyDataSetChanged();
				});
				callDialog.SetNegativeButton("Cancel", delegate { });
				callDialog.Show();
			}
		}

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                if (data != null)
                {
                    changeIsComing = true;
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

        public override void OnBackPressed()
        {
            if (changeIsComing)
            {
                var callDialog = new AlertDialog.Builder(this);
                callDialog.SetMessage("Order has been modified, discard changes?");
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