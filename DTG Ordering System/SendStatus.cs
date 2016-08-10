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
    public class SendStatus
    {
        public string order_id;
        public string send_status;

        public SendStatus(string order_id, string send_status)
        {
            this.order_id = order_id;
            this.send_status = send_status;
        }
    }
}