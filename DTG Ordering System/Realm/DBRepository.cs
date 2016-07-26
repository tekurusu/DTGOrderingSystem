using System;
using System.Data;
using System.IO;
using Realms;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace DTG_Ordering_System
{
	public class DBRepository
	{
		private Realm realm;
		private static string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "default.realm");
		RealmConfiguration config = new RealmConfiguration(dbPath, true);

        public void createDB()
		{
			Realm.GetInstance(config);
		}

		public void insertItem(string itemName, string itemUnit, Category category)
		{
			realm = Realm.GetInstance(config);

			using (var transaction = realm.BeginWrite())
			{
				var item = realm.CreateObject<Item>();
				string UUID = Guid.NewGuid().ToString();

				item.Id = UUID;
				item.Name = itemName;
				item.Unit = itemUnit;
				item.Category = category;
				category.Items.Add(item);

				transaction.Commit();
			}
		}

        public string insertCategory(string categoryName)
        {
			realm = Realm.GetInstance(config);
            string UUID;

			using (var transaction = realm.BeginWrite())
			{
				var category = realm.CreateObject<Category>();

				UUID = Guid.NewGuid().ToString();

				category.Id = UUID;
				category.Name = categoryName;

				transaction.Commit();
			}

            return UUID;
		}

        public Category getCategory(string categoryId)
        {
			return Realm.GetInstance(config).All<Category>().Where(c => c.Id == categoryId).First();
        }

        public Item getItem(string itemId)
        {
            return Realm.GetInstance(config).All<Item>().Where(c => c.Id == itemId).First();
        }

		public List<Item> getAllItems(string categoryId)
		{
			List<Item> items = new List<Item>();
			var cat = getCategory(categoryId).Items;

            foreach (var c in cat)
			{
                items.Add(c);
			}

			return items;         
		}

		public List<Category> getAllCategories()
		{
			realm = Realm.GetInstance(config);

			var allCategories = realm.All<Category>().OrderBy(i => i.Name);
			List<Category> categories = new List<Category>();

			foreach (var cat in allCategories)
			{
				categories.Add(cat);
			}
			return categories;
		}

		public void setQuantity(string itemId, int quantity)
		{ 
			realm = Realm.GetInstance(config);

			using (var transaction = realm.BeginWrite())
			{
				var someItem = realm.All<Item>().Where(i => i.Id == itemId).First();
				someItem.Quantity = quantity;

				transaction.Commit();
			}
		}

		public void insertOrderedItem(int quantity, Item item, Order order)
		{
			realm = Realm.GetInstance(config);

			using (var transaction = realm.BeginWrite())
			{
				var orderedItem = realm.CreateObject<OrderedItem>();
				orderedItem.Quantity = quantity;
				orderedItem.Item = item;
				orderedItem.Order = order;
				item.OrderedItems.Add(orderedItem);
				order.OrderedItems.Add(orderedItem);

				transaction.Commit();
			}
		}

		public string insertOrder(string deliveryDate)
		{
			realm = Realm.GetInstance(config);

			using (var transaction = realm.BeginWrite())
			{
				var order = realm.CreateObject<Order>();
				string UUID = Guid.NewGuid().ToString();

				order.Id = UUID;
				order.DeliveryDate = deliveryDate;
				order.HasSent = false;

				transaction.Commit();

				return order.Id;
			}
		}

		public void sendOrder(string orderId)
		{
			realm = Realm.GetInstance(config);

			using (var transaction = realm.BeginWrite())
			{
				var someOrder = realm.All<Order>().Where(o => o.Id == orderId).First();
				someOrder.HasSent = true;

				transaction.Commit();
			}
		}

		public Order getOrder(string orderId)
		{
			return Realm.GetInstance(config).All<Order>().Where(o => o.Id == orderId).First();
		}

		//public List<OrderedItem> getAllOrdersItems(string orderId)
		//{
		//	List<OrderedItem> orderedItems = new List<OrderedItem>();
		//	var o = getOrder(orderId).OrderedItems;

		//	foreach (var oI in o)
		//	{
		//		orderedItems.Add(oI);
		//	}

		//	return orderedItems;
		//}

		public string getAllOrderedItems(string orderId)
		{
			var o = getOrder(orderId).OrderedItems;
			string orderedItems = "";
			orderedItems += "Retrieving Ordered Items from Order";

			foreach (var oI in o)
			{
				orderedItems += String.Format("\n {0} | {1}", oI.Item.Name, oI.Item.Quantity);
			}

			return orderedItems;
		}

		public string getAllOrders()
		{
			realm = Realm.GetInstance(config);

			var allOrders = realm.All<Order>();
			string orders = "";
			orders += "Retrieving All Orders";

			foreach (var o in allOrders)
			{
				orders += String.Format("\n {0}", o.Id);
			}
			return orders;
		}

        public void deleteDB()
        {
			realm = Realm.GetInstance(config);

			using (var transaction = realm.BeginWrite())
			{
				realm.RemoveAll();
				transaction.Commit();
			}
			realm.Close();
		}

        public void syncDB() //temporary load of database files :))
        {
			if (!File.Exists(dbPath))
			{
				createDB();

				string meat = insertCategory("Meat");
				string spices = insertCategory("Spices");
				string others = insertCategory("Others");

				insertItem("Chicken", "kilos", getCategory(meat));
				insertItem("Beef", "cows", getCategory(meat));
				insertItem("Pork", "pigs", getCategory(meat));
				insertItem("Paprika", "manyak", getCategory(spices));
				insertItem("Salt", "mats", getCategory(spices));
				insertItem("Sugar", "subjects", getCategory(spices));
				insertItem("Broom", "top 1", getCategory(others));
				insertItem("Fan", "grad school", getCategory(others));
				insertItem("Water", "lightning", getCategory(others));
			}
        }
	}
}

