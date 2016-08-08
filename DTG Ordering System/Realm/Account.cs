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
using Realms;

namespace DTG_Ordering_System
{
    public class Account : RealmObject
    {
        [ObjectId]
        public string BranchId { get; set; }
        public string Branch { get; set; }
        public string Password { get; set; }
        public RealmList<Order> Orders { get; }
    }
}