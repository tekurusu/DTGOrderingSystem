using System;
using Realms;

namespace DTG_Ordering_System
{
    public class OrderedItem : RealmObject
    {
       	public int Quantity { get; set; }

        //public Order Order { get; set; }
        //public Item Item { get; set; }
		public string OrderId { get; set; }
		public string ItemId { get; set; }
    }
}