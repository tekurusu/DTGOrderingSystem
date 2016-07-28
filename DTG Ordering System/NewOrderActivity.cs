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
        private static List<ParentCategory> addedCategories = new List<ParentCategory>();
        private ExpandableListView mListView;
        private ExpandableNewOrderAdapter adapter;
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

            mListView = FindViewById<ExpandableListView>(Resource.Id.selectedItemsListView);
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

            addedCategories.Clear();
            items.Clear();
            adapter = new ExpandableNewOrderAdapter(this, addedCategories);
            mListView.SetAdapter(adapter);

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

                dateHolder = DateTime.Parse(order.DeliveryDate);
                deliveryDate.Text = String.Format("{0:dd MMM yy}", DateTime.Parse(order.DeliveryDate));
                sendButton.Enabled = true;

                foreach (OrderedItem oi in orderedItems)
                {
                    items.Add(oi.Item);
                }

                var temp = items.GroupBy(x => x.Category.Id);
                foreach(var e in temp)
                {
                    List<Item> manyak = new List<Item>();
                    foreach (Item i in e)
                    {
                        manyak.Add(i);
                    }
                    addedCategories.Add(new ParentCategory(e.First().Category.Name, manyak));
                }               

                adapter.NotifyDataSetChanged();
            }

            editDate.Click += (object sender, EventArgs e) =>
            {
                DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    deliveryDate.Text = String.Format("{0:dd MMM yy}", time);
                    changeIsComing = true;
                    if (items.Count != 0) saveButton.Enabled = true;
                });
                Bundle args = new Bundle();
                args.PutInt("year", dateHolder.Year);
                args.PutInt("month", dateHolder.Month);
                args.PutInt("day", dateHolder.Day);

                frag.Arguments = args;
                frag.Show(FragmentManager, DatePickerFragment.TAG);
            };
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
        void DeleteItem_OnLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
            long listposition = mListView.GetExpandableListPosition(e.Position);
            int childPosition = ExpandableListView.GetPackedPositionChild(listposition);
            int groupPosition = ExpandableListView.GetPackedPositionGroup(listposition);

            if (ExpandableListView.GetPackedPositionType(listposition) == PackedPositionType.Child)
            {
                var callDialog = new AlertDialog.Builder(this);
                callDialog.SetMessage("Delete " + addedCategories[groupPosition].Items[childPosition].Name + "?");
                callDialog.SetNeutralButton("Delete", delegate
                {
                    addedCategories[groupPosition].Items.RemoveAt(childPosition);
                    adapter.NotifyDataSetChanged();
                });
                callDialog.SetNegativeButton("Cancel", delegate { });
                callDialog.Show();
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            string categoryName;

            if (resultCode == Result.Ok)
            {
                if (data != null)
                {
                    saveButton.Enabled = true;
                    sendButton.Enabled = true;
                    var message = data.GetStringExtra("addedItems");
                    List<Item> addedItems = JsonConvert.DeserializeObject<List<Item>>(message);
                    categoryName = addedItems[0].Category.Name;
                    ParentCategory pc = new ParentCategory(categoryName, addedItems);
                    if (addedCategories.Exists(category => category.Name == categoryName) == false) //check if the category is already in the list
                    {
                        addedCategories.Add(pc);
                        foreach(Item i in pc.Items)
                        {
                            items.Add(i);
                        }
                    }

                    else    //if category already exists..update all the item quantities
                    {
                        ParentCategory tempCategory = addedCategories.Find(category => category.Name == pc.Name);

                        foreach (Item i in pc.Items)
                        {
                            //if item exists, update quantity. else add item to category.items
                            if (tempCategory.Items.ToList().Exists(item => item.Id == i.Id) == true)
                            {
                                tempCategory.Items.ToList().Find(item => item.Id == i.Id).Quantity += i.Quantity;
                                //items.Find(item => item.Id == i.Id).Quantity += i.Quantity;                                
                            }

                            else
                            {
                                tempCategory.Items.Add(i);
                                items.Add(i);
                            }
                        }
                    }

                    adapter.NotifyDataSetChanged();
                    changeIsComing = true;
                }
            }			
        }

        public override void OnBackPressed()
        {
            if (changeIsComing)
            {
                var callDialog = new AlertDialog.Builder(this);
                callDialog.SetMessage("Discard changes for this order?");
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