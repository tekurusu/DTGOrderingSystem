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
using Android.Preferences;
using System.ServiceModel;
using Newtonsoft.Json;

namespace DTG_Ordering_System
{
    
    [Activity(Label = "DTG Ordering System", Icon = "@drawable/icon")]
    public class LoginActivity : Activity
    {
        private Button loginButton;
        private Spinner userSpinner;
        private EditText passwordText;
        DBRepository dbr = new DBRepository();
        private Button syncButton;
        string branchId;
        private Service1Client _client;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            dbr = new DBRepository();

            base.OnCreate(savedInstanceState);
            this.RequestWindowFeature(WindowFeatures.NoTitle);
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            branchId = prefs.GetString("branchId", null);
            if (branchId != null)
            {
                Intent intent = new Intent(this, typeof(OrdersActivity));
                StartActivity(intent);
            }

            SetContentView(Resource.Layout.loginScreen);
            loginButton = FindViewById<Button>(Resource.Id.loginButton);
            userSpinner = FindViewById<Spinner>(Resource.Id.userSpinner);
            passwordText = FindViewById<EditText>(Resource.Id.passwordText);
            syncButton = FindViewById<Button>(Resource.Id.syncButton);            
            var accounts2 = dbr.getAllAccounts();
            string[] accounts = new string[accounts2.Count() + 1];
            accounts[0] = "Branch";
            for (int x = 1; x < accounts2.Count() + 1; x++)
            {
                
                accounts[x] = accounts2[x - 1].Branch;
            }
            
            ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, accounts);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            userSpinner.Adapter = adapter;
            
            loginButton.Click += delegate
            {

                if (dbr.authenticate(userSpinner.SelectedItem.ToString(), passwordText.Text) >= 1)
                {
                    Intent intent = new Intent(this, typeof(OrdersActivity));
                    //intent.PutExtra("branchId", accounts2[userSpinner.SelectedItemPosition].BranchId.ToString());

                    //ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                    ISharedPreferencesEditor editor = prefs.Edit();
                    editor.PutString("branchId", accounts2[userSpinner.SelectedItemPosition - 1].BranchId.ToString());
                    editor.Apply();

                    StartActivity(intent);
                    //Toast.MakeText(this, accounts2[userSpinner.SelectedItemPosition].BranchId.ToString(), ToastLength.Long).Show();
                    //StartActivityForResult(intent, 2);
                }
                else
                {
                    Toast.MakeText(this, "Sorry, we can't log you in with what you entered, please try again.", ToastLength.Long).Show();
                }
            };

            syncButton.Click += (object sender, EventArgs e) =>
            {
                var progressDialog = ProgressDialog.Show(this, "Please wait...", "Syncing Database...", true);
                new Thread(new ThreadStart(delegate
                {
                    dbr.syncDB();
                    InitializeService1Client();
                    _client.getAllCategoriesAsync();

                    //hide progress dialogue
                    RunOnUiThread(() => progressDialog.Hide());
                })).Start();
                adapter.NotifyDataSetChanged();
            };
            //Create your application here
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


        private void InitializeService1Client()
        {
            BasicHttpBinding binding = CreateBasicHttp();

            _client = new Service1Client(binding, dbr.getIP());
            _client.getAllCategoriesCompleted += _client_getAllCategoriesCompleted;
            _client.getAllItemsCompleted += _client_getAllItemsCompleted;
        }

        private void _client_getAllItemsCompleted(object sender, getAllItemsCompletedEventArgs e)
        {
            List<Item> items = JsonConvert.DeserializeObject<List<Item>>(e.Result);
            items.Sort((x, y) => x.Name.CompareTo(y.Name));
            foreach (Item i in items)
            {
                dbr.insertItem(i.Id, i.Name, i.Unit, dbr.getCategory(i.Category_Id));
            }

            //Category c = dbr.getCategory(items[0].Category_Id);
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
                msg = "Successfully synced items";
            }
            RunOnUiThread(() => Toast.MakeText(this, msg, ToastLength.Long).Show());
        }

        private void _client_getAllCategoriesCompleted(object sender, getAllCategoriesCompletedEventArgs e)
        {
            List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(e.Result);
            foreach (Category c in categories)
            {
                dbr.insertCategory(c.Id, c.Name);
            }

            _client.getAllItemsAsync();
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