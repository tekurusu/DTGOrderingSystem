using System;
using System.Data;
using System.IO;
using SQLite;
using System.Collections.Generic;
using Android.Content;

namespace DTG_Ordering_System
{
	public class DBRepository
	{
        // code to create SQL connection
        //public void establishSQLConnection()
        //{
        //	//where the file will be created on which folder and establishing an SQL connection
        //	string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dtgapp.db3");
        //	var db = new SQLiteConnection(dbPath);
        //}

        //code to create the database
        public void CreateDB()
		{
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dtgapp.db3");
			var db = new SQLiteConnection(dbPath);
		}

		//code to create a table
		public void CreateTable()
		{
			//where the file will be created on which folder and establishing an SQL connection
			string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dtgapp.db3");
			var db = new SQLiteConnection(dbPath);
            
            db.CreateTable<Item>();
            db.CreateTable<Category>();
		}

		//code to insert data
		public void insertItem(string itemName, string itemUnit, Category category)
		{ 
			//where the file will be created on which folder and establishing an SQL connection
			string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dtgapp.db3");
			var db = new SQLiteConnection(dbPath);

			Item item = new Item();
			item.Name = itemName;
			item.Unit = itemUnit;
            //item.CategoryId = category.Id;

            db.Insert(item);
		}

        public void insertCategory(string name)
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dtgapp.db3");
            var db = new SQLiteConnection(dbPath);

            Category category = new Category();
            category.Name = name;

            db.Insert(category);
        }

        public Category getCategory(string name)
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dtgapp.db3");
            var db = new SQLiteConnection(dbPath);
            Category cat = new Category();

            var table = db.Table<Category>();
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
		public List<Item> GetAllData()
		{
			//where the file will be created on which folder and establishing an SQL connection
			string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dtgapp.db3");
			var db = new SQLiteConnection(dbPath);

			List<Item> items = new List<Item>();
			var table = db.Table<Item>();
			foreach (var item in table)
			{
                items.Add(item);
			}

			return items;
		}

		//code to retrieve specific data using ORM
		public string GetDataById(int id)
		{
			try
			{
				//where the file will be created on which folder and establishing an SQL connection
				string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dtgapp.db3");
				var db = new SQLiteConnection(dbPath);

				//get is a method which is going to retrieve data based on the Primary Key
				var item = db.Get<Item>(id);

				return item.Name + " | " + item.Unit;
			}
			catch (Exception ex)
			{
				return "Error: " + ex.Message;
			}
		}

		//code to retrieve specific attribute using ORM - Item Name
		public string GetItemNameById(int id)
		{
			try
			{
				//where the file will be created on which folder and establishing an SQL connection
				string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dtgapp.db3");
				var db = new SQLiteConnection(dbPath);

				//get is a method which is going to retrieve data based on the Primary Key
				var item = db.Get<Item>(id);

				return item.Name;
			}
			catch (Exception ex)
			{
				return "Error: " + ex.Message;
			}
		}

		//code to retrieve specific attribute using ORM - Item Quantity
		public string GetItemQuantityById(int id)
		{
			try
			{
				//where the file will be created on which folder and establishing an SQL connection
				string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dtgapp.db3");
				var db = new SQLiteConnection(dbPath);

				//get is a method which is going to retrieve data based on the Primary Key
				var item = db.Get<Item>(id);

				return item.Unit.ToString();
			}
			catch (Exception ex)
			{
				return "Error: " + ex.Message;
			}
		}

		//code to update data using ORM
		public string UpdateData(int id, string name, string unit)
		{ 
			try
			{
				//where the file will be created on which folder and establishing an SQL connection
				string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dtgapp.db3");
				var db = new SQLiteConnection(dbPath);

				var item = db.Get<Item>(id);
				item.Name = name;
				item.Unit = unit;

				db.Update(item);

				return "Item updated...";
			}
			catch (Exception ex)
			{
				return "Error: " + ex.Message;
			}
		}

		//code to delete data
		public string DeleteItem(int id)
		{
			try
			{
				//where the file will be created on which folder and establishing an SQL connection
				string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dtgapp.db3");
				var db = new SQLiteConnection(dbPath);

				string tempName = GetItemNameById(id);

				var item = db.Get<Item>(id);
				db.Delete(item);

				return String.Format("Item: {0} deleted successfully...", tempName);
			}
			catch (Exception ex)
			{
				return "Error: " + ex.Message;
			}
		}

        public void deleteDB()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "dtgapp.db3");
            var db = new SQLiteConnection(dbPath);

            db.DeleteAll<Item>();
            //db.DeleteAll<Category>();
        }
	}
}

