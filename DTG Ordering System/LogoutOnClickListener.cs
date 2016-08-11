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
using Android.Preferences;


namespace DTG_Ordering_System
{
    public class LogoutClickListener : Java.Lang.Object, View.IOnClickListener
    {
        private Activity activity;
        public LogoutClickListener(Activity activity)
        {
            this.activity = activity;
        }
        public void OnClick(View v)
        {
            var callDialog = new AlertDialog.Builder(activity);
            callDialog.SetMessage("Are you sure you want to logout?");
            callDialog.SetNeutralButton("Yes", delegate
            {
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(activity);
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.Clear();
                editor.Apply();

                Intent intent = new Intent(activity.ApplicationContext, typeof(LoginActivity));
                activity.StartActivityForResult(intent, 1);
            });
            callDialog.SetNegativeButton("No", delegate { });
            callDialog.Show();
        }
    }
}