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
using Android.Preferences;
using System.ServiceModel;

namespace DTG_Ordering_System
{
    [Activity(Label = "New Order", Icon = "@drawable/icon")]
    public class NewOrderActivity : Activity
    {
        private static List<Item> items = new List<Item>();
        private static List<OrderedItem> orderedItems = new List<OrderedItem>();
        private static List<ParentCategory> addedCategories = new List<ParentCategory>();
        private static Dictionary<string, int> addedQuantities = new Dictionary<string, int>();
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
        private string branchId;
        ISharedPreferences prefs;

        public static readonly EndpointAddress EndPoint = new EndpointAddress("http://192.168.1.7:61606/Service1.svc");
        private Service1Client _client;
        

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.newOrder);
            prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            branchId = prefs.GetString("branchId", null);
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
            String dateNow = String.Format("{0:dd MMM yyyy}", now);
            deliveryDate.Text = dateNow;            

            addedCategories.Clear();
            items.Clear();
            addedQuantities.Clear();
            adapter = new ExpandableNewOrderAdapter(this, addedCategories, addedQuantities);
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
				//***FOR EDIT****//

				Order order = dbr.getOrder(Intent.GetStringExtra("orderId"));
				List<OrderedItem> orderedItems = dbr.getAllOrderedItems(order.Id);

				dateHolder = DateTime.Parse(order.DeliveryDate);
				deliveryDate.Text = String.Format("{0:dd MMM yyyy}", DateTime.Parse(order.DeliveryDate));
				sendButton.Enabled = true;

				//Toast.MakeText(this, addedQuantities.Count.ToString(), ToastLength.Long).Show();
				foreach (OrderedItem oi in orderedItems)
				{
					dbr = new DBRepository();

					items.Add(dbr.getItem(oi.ItemId));

					addedQuantities.Add(oi.ItemId, oi.Quantity);
				}

				var temp = items.GroupBy(x => x.Category.Id);
				foreach (var e in temp)
				{
					List<Item> listHolder = new List<Item>();
					foreach (Item i in e)
					{
						listHolder.Add(i);
					}
					addedCategories.Add(new ParentCategory(e.First().Category.Name, listHolder));
				}

				adapter.NotifyDataSetChanged();

				if ((Intent.GetBooleanExtra("hasSent", false) == false) || (Intent.GetBooleanExtra("replacement", false) == true))
				{
					editDate.Visibility = ViewStates.Visible;
					saveButton.Visibility = ViewStates.Visible;
					sendButton.Visibility = ViewStates.Visible;
					addItemsButton.Visibility = ViewStates.Visible;
				}
				else
				{
					editDate.Visibility = ViewStates.Gone;
					saveButton.Visibility = ViewStates.Gone;
					sendButton.Visibility = ViewStates.Gone;
					addItemsButton.Visibility = ViewStates.Gone;
				}
            }

            editDate.Click += (object sender, EventArgs e) =>
            {
                DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    deliveryDate.Text = String.Format("{0:dd MMM yyyy}", time);
                    dateHolder = time;
                    if (items.Count != 0)
                    {
                        saveButton.Enabled = true;
						sendButton.Enabled = true;
                        changeIsComing = true;

                    }
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
			var callDialog = new AlertDialog.Builder(this);
			callDialog.SetMessage("Are you sure you want to save this order as a draft?");
			callDialog.SetNeutralButton("OK", delegate
			{
				dbr = new DBRepository();
                string orderId;
                string branchId = prefs.GetString("branchId", null);
                if (Intent.GetStringExtra("orderId") == null)
                {
					orderId = dbr.insertOrder(deliveryDate.Text, false, branchId);
                    dbr.insertOrderedItems(items, orderId, addedQuantities);
                }
                else
                {
                    orderId = Intent.GetStringExtra("orderId");
					dbr.updateOrder(orderId, deliveryDate.Text, false);
					dbr.updateOrderedItems(addedCategories, orderId, addedQuantities);
                }

                Intent intent = new Intent(ApplicationContext, typeof(OrdersActivity));
				intent.PutExtra("OrderId", orderId);
				StartActivityForResult(intent, 1);
                //SetResult(Result.Ok, intent);
                //Finish();

                items.Clear();
				adapter.NotifyDataSetChanged();
			});
			callDialog.SetNegativeButton("Cancel", delegate { });
			callDialog.Show();
		}

		void SendButton_OnClick(object sender, EventArgs e)
		{
            //var callDialog = new AlertDialog.Builder(this);
            //callDialog.SetMessage("Are you sure you want to send this order?");
            //callDialog.SetNeutralButton("OK", delegate
            //{
            //    dbr = new DBRepository();
            //    string orderId;
            //    string branchId = prefs.GetString("branchId", null);
            //    if (Intent.GetStringExtra("orderId") == null)
            //    {
            //        orderId = dbr.insertOrder(deliveryDate.Text, true, branchId);
            //        dbr.insertOrderedItems(items, orderId, addedQuantities);
            //    }
            //    else
            //    {
            //        orderId = Intent.GetStringExtra("orderId");
            //        dbr.updateOrder(orderId, deliveryDate.Text, true);
            //        dbr.updateOrderedItems(addedCategories, orderId, addedQuantities);
            //    }

            //    Intent intent = new Intent(ApplicationContext, typeof(OrdersActivity));
            //    intent.PutExtra("OrderId", orderId);
            //    StartActivityForResult(intent, 1);

            //    items.Clear();
            //    adapter.NotifyDataSetChanged();
            //});
            //callDialog.SetNegativeButton("Cancel", delegate { });
            //callDialog.Show();

            InitializeService1Client();
            _client.sendOrderAsync(Convert.ToDateTime(deliveryDate.Text));
            
        }

        void DeleteItem_OnLongClick(object sender, AdapterView.ItemLongClickEventArgs e)
        {
			if (Intent.GetBooleanExtra("hasSent", false) == false)
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
						if (Intent.GetStringExtra("orderId") != null)
						{
							addedQuantities.Remove(addedCategories[groupPosition].Items[childPosition].Id);
                            
							addedCategories[groupPosition].Items.RemoveAt(childPosition);
                            if (addedCategories[groupPosition].Items.Count == 0)
                            {
                                addedCategories.RemoveAt(groupPosition);
                            }
							adapter.NotifyDataSetChanged();
						}
						else
						{
							Item searchedItem = items.Find(x => x.Id == addedCategories[groupPosition].Items[childPosition].Id);
							items.Remove(searchedItem);
							addedQuantities.Remove(addedCategories[groupPosition].Items[childPosition].Id);
							addedCategories[groupPosition].Items.RemoveAt(childPosition);
                            if (addedCategories[groupPosition].Items.Count == 0)
                            {
                                addedCategories.RemoveAt(groupPosition);
                            }
                            adapter.NotifyDataSetChanged();
						}

						changeIsComing = true;

						if (addedCategories.Count != 0)
						{
							saveButton.Enabled = true;
							sendButton.Enabled = true;
							adapter.NotifyDataSetChanged();
						}
						else
						{
							saveButton.Enabled = false;
							sendButton.Enabled = false;
							adapter.NotifyDataSetChanged();
						}
					});
					callDialog.SetNegativeButton("Cancel", delegate { });
					callDialog.Show();
				}
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
                    var messageQuantity = data.GetStringExtra("addedQuantities");
                    //Toast.MakeText(this, messageQuantity.ToString(), ToastLength.Long).Show();

                    List<Item> addedItems = JsonConvert.DeserializeObject<List<Item>>(message);
                    Dictionary<string, int> tempQuantities = JsonConvert.DeserializeObject<Dictionary<string, int>>(messageQuantity);

                    foreach(KeyValuePair<string, int> x in tempQuantities)
                    {
                        if (addedQuantities.ContainsKey(x.Key) == true)
                        {
                            addedQuantities[x.Key] += tempQuantities[x.Key];
                        }
                        else
                        {
                            addedQuantities.Add(x.Key, x.Value);
                        }
                    }

                    categoryName = addedItems[0].Category.Name;
                    ParentCategory pc = new ParentCategory(categoryName, addedItems);
                    if (addedCategories.Exists(category => category.Name == categoryName) == false) //check if the category is already in the list
                    {
                        addedCategories.Add(pc);

                        foreach(Item i in addedItems) //for items..
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
                            if (tempCategory.Items.ToList().Exists(item => item.Id == i.Id) == false)
                            {
                                //tempCategory.Items.ToList().Find(item => item.Id == i.Id).Quantity += i.Quantity;
                                //items.Find(item => item.Id == i.Id).Quantity += i.Quantity;           
                                tempCategory.Items.Add(i);
                                items.Add(i);
                            }
                        }
                    }

					addedCategories.Sort((x, y) => x.Name.CompareTo(y.Name));
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

        private void InitializeService1Client()
        {
            BasicHttpBinding binding = CreateBasicHttp();

            _client = new Service1Client(binding, EndPoint);
            _client.sendOrderCompleted += _client_sendOrderCompleted;
        }

        private void _client_sendOrderCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            string msg = null;

            if (e.Error != null)
            {
                msg = e.Error.Message;
            }
            else if (e.Cancelled)
            {
                msg = "Request was cancelled.";
            }
            else
            {
                msg = "Successfully sent order";
            }
            RunOnUiThread(() => Toast.MakeText(this, msg, ToastLength.Long).Show());
        }

        private static BasicHttpBinding CreateBasicHttp()
        {
            BasicHttpBinding binding = new BasicHttpBinding
            {
                Name = "basicHttpBinding",
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647
            };
            TimeSpan timeout = new TimeSpan(0, 0, 30);
            binding.SendTimeout = timeout;
            binding.OpenTimeout = timeout;
            binding.ReceiveTimeout = timeout;
            return binding;
        }
    }
}