using System;
using Realms;

namespace DTG_Ordering_System
{
    public class Order : RealmObject
    {
        [ObjectId]
        public string Id { get; set; }
        public string DeliveryDate { get; set; }
        public bool HasSent { get; set; }
        public RealmList<OrderedItem> OrderedItems { get; }
    }
}