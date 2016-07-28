using System;
using Realms;

namespace DTG_Ordering_System
{
	public class Item : RealmObject
    {
        [ObjectId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        //public int Quantity { get; set; }

        public Category Category { get; set; }
        public RealmList<OrderedItem> OrderedItems { get; }
    }
}