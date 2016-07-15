using System;
using Realms;

namespace DTG_Ordering_System
{
	public class Category : RealmObject
    {
		[ObjectId]
        public string Id { get; set; }
        public string Name { get; set; }
		public RealmList<Item> Items { get; }
	}
}