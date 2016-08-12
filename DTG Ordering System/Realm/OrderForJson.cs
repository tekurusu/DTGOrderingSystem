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
    public class OrderForJson
    {
        public string Id { get; set; }
        public string DeliveryDate { get; set; }
        public bool HasSent;
        public string BranchId { get; set; }

        public OrderForJson (string id, string deliveryDate, bool hasSent, string branchId)
        {
            Id = id;
            DeliveryDate = deliveryDate;
            HasSent = hasSent;
            BranchId = branchId;
        }
    }
}