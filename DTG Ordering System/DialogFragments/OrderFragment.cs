using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace DTG_Ordering_System
{
    public class OrderFragment : DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            int index = Activity.Intent.GetIntExtra("Index", -1);
            LinearLayout lin = new LinearLayout(this.Context);
            Dialog dialog = new Dialog(this.Context);
            dialog.SetTitle(index.ToString());
            Button b = new Button(this.Context);
            Button c = new Button(this.Context);
            string edit = "Edit";
            string delete = "Delete";
            b.SetText(edit, TextView.BufferType.Normal);
            c.SetText(delete, TextView.BufferType.Normal);
            b.Click += (object sender, EventArgs e) =>
            {

                //Intent intent = new Intent(Activity.ApplicationContext, typeof(NewOrderActivity));
                ////intent.PutExtra("Index", args);
                //StartActivity(intent);
                Console.WriteLine("wew");
            };
            lin.AddView(b);
            lin.AddView(c);
            return lin;
        }
    }
}