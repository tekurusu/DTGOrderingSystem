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

namespace DTG_Ordering_System
{
    [Activity(Label = "Log In", Icon = "@drawable/icon")]
    public class LoginActivity : Activity
    {
        private Button loginButton;
        private Spinner userSpinner;
        private EditText passwordText;
        DBRepository dbr = new DBRepository();
        private Button syncButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            dbr = new DBRepository();
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.loginScreen);
            loginButton = FindViewById<Button>(Resource.Id.loginButton);
            userSpinner = FindViewById<Spinner>(Resource.Id.userSpinner);
            passwordText = FindViewById<EditText>(Resource.Id.passwordText);
            syncButton = FindViewById<Button>(Resource.Id.syncButton);
            
            var accounts2 = dbr.getAllAccounts();
            String[] accounts = new String[accounts2.Count()];
            for (int x = 0; x < accounts2.Count(); x++)
            {
                accounts[x] = accounts2[x].Branch;
            }
            ArrayAdapter<String> adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleSpinnerItem, accounts);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            userSpinner.Adapter = adapter;
            
            loginButton.Click += delegate
            {

                if (dbr.authenticate(userSpinner.SelectedItem.ToString(), passwordText.Text) >= 1)
                {
                    Intent intent = new Intent(this, typeof(OrdersActivity));
                    intent.PutExtra("branchId", accounts2[userSpinner.SelectedItemPosition].BranchId.ToString());
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
                    //hide progress dialogue
                    RunOnUiThread(() => progressDialog.Hide());
                })).Start();

                adapter.NotifyDataSetChanged();
            };
            //Create your application here
        }
    }
}