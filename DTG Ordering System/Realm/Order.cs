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

namespace DTG_Ordering_System
{
    class Order
    {
        public int Id { get; set; }
        public string DeliveryDate { get; set; }
        public bool HasSent { get; set; }
    }
}