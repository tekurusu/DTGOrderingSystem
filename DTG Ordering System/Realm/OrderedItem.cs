using System;
using Realms;

namespace DTG_Ordering_System
{
    public class OrderedItem : RealmObject
    {
        [ObjectId]
        //public string Id { get; set; }
        public int Quantity { get; set; }

        public Order Order { get; set; }
        public Item Item { get; set; }
    }
}