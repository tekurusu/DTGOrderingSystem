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
			else
			{
				dbr.loadAccounts();
			}

            SetContentView(Resource.Layout.loginScreen);
            loginButton = FindViewById<Button>(Resource.Id.loginButton);
            userSpinner = FindViewById<Spinner>(Resource.Id.userSpinner);
            passwordText = FindViewById<EditText>(Resource.Id.passwordText);   
            var accounts2 = dbr.getAllAccounts();

            List<String> accounts = new List<String>();

            accounts.Add("<Select Branch>");
            foreach (var i in accounts2)
            {
                accounts.Add(i.Branch);
            }

            SpinnerAdapter adapter = new SpinnerAdapter(this, accounts, this);

            userSpinner.Adapter = adapter;
            userSpinner.SetSelection(0);           

            loginButton.Click += delegate
            {

                if (dbr.authenticate(userSpinner.SelectedItem.ToString(), passwordText.Text) >= 1)
                {
                    Intent intent = new Intent(this, typeof(OrdersActivity));
                    //intent.PutExtra("branchId", accounts2[userSpinner.SelectedItemPosition].BranchId.ToString());

                    //ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                    ISharedPreferencesEditor editor = prefs.Edit();
                    editor.PutString("branchId", accounts2[userSpinner.SelectedItemPosition].BranchId.ToString());
                    editor.Apply();

                    StartActivity(intent);
                    //Toast.MakeText(this, accounts2[userSpinner.SelectedItemPosition].BranchId.ToString(), ToastLength.Long).Show();
                    //StartActivityForResult(intent, 2);
                }
                else
                {
                    Toast.MakeText(this, "Invalid branch or password. Please try again.", ToastLength.Long).Show();
                }
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