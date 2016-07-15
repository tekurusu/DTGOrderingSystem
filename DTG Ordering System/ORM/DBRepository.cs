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
        public void CreateDB()
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

        public void insertCategory(string categoryName)
        {
			realm = Realm.GetInstance();

			using (var transaction = realm.BeginWrite())
			{
				var category = realm.CreateObject<Category>();

				string UUID = Guid.NewGuid().ToString();

				category.Id = UUID;
				category.Name = categoryName;

				transaction.Commit();
			}

		}

		//code to get category using category name
        public Category getCategory(string name)
        {
			realm = Realm.GetInstance();

			Category cat = new Category();

			var table = realm.All<Category>();
            foreach (var cate in table)
            {
                if (cate.Name == name)
                {
                    cat = cate;
                }
            }
            return cat;
        }

		//code to retrieve all data
		public List<Item> GetAllItems(string categoryId)
		{
			realm = Realm.GetInstance();

			List<Item> items = new List<Item>();

			var cat = realm.All<Category>().Where(c => c.Id == categoryId);

			foreach (var cate in cat)
			{
				foreach (var i in cate.Items)
				{
					items.Add(i);
				}
			}

			return items;
		}

		public List<Category> GetAllCategories()
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

		//code to set quantity of an item
		public void SetQuantity(string itemId, int quantity)
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
	}
}

