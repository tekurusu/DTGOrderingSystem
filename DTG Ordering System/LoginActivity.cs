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
    }
}