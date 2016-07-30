using System;
using Realms;

namespace DTG_Ordering_System
{
    public class OrderedItem : RealmObject
    {
		public string OrderId { get; set; }
		public string ItemId { get; set; }
		public int Quantity { get; set; }
	}
}