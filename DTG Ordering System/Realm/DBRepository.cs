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

        //code to create the database
        public void createDB()
		{
			realm = Realm.GetInstance();
		}

		//code to insert data
		public void insertItem(string itemName, string itemUnit, Category category)
		{
			realm = Realm.GetInstance();

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
			realm = Realm.GetInstance();
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
            return Realm.GetInstance().All<Category>().Where(c => c.Id == categoryId).First();
        }

        public Item getItem(string guid)
        {
            return Realm.GetInstance().All<Item>().Where(c => c.Id == guid).First();
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
			realm = Realm.GetInstance();

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
			realm = Realm.GetInstance();

			using (var transaction = realm.BeginWrite())
			{
				var someItem = realm.All<Item>().Where(i => i.Id == itemId).First();
				someItem.Quantity = quantity;

				transaction.Commit();
			}
		}

        public void deleteDB()
        {
			realm = Realm.GetInstance();

			using (var transaction = realm.BeginWrite())
			{
				realm.RemoveAll();
				transaction.Commit();
			}
			realm.Close();
		}

        public void syncDB() //temporary load of database files :))
        {
            deleteDB();
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

