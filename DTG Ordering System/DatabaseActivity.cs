using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DTG_Ordering_System
{
    [Activity(Label = "Home Screen", Icon = "@drawable/icon")]
    public class DatabaseActivity : Activity
    {
        private Button addOrder;
        private Button loadDB;
        private Button deleteDB;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.databaseScreen);

            addOrder = FindViewById<Button>(Resource.Id.addOrder);
            loadDB = FindViewById<Button>(Resource.Id.loadDB);
            deleteDB = FindViewById<Button>(Resource.Id.deleteDB);

            addOrder.Click += (object sender, EventArgs e) =>
            {
				StartActivity(typeof(CategoriesActivity));
            };

            loadDB.Click += (object sender, EventArgs e) =>
            {
                DBRepository dbr = new DBRepository();
                dbr.createDB();

                dbr.insertCategory("Meat");
				dbr.insertCategory("Spices");
				dbr.insertCategory("Others");

                dbr.insertItem("Chicken", "kilos", dbr.getCategory("Meat"));
                dbr.insertItem("Beef", "cows", dbr.getCategory("Meat"));
                dbr.insertItem("Pork", "pigs", dbr.getCategory("Meat"));
                dbr.insertItem("Paprika", "manyak", dbr.getCategory("Spices"));
                dbr.insertItem("Salt", "mats", dbr.getCategory("Spices"));
                dbr.insertItem("Sugar", "subjects", dbr.getCategory("Spices"));
                dbr.insertItem("Broom", "top 1", dbr.getCategory("Others"));
                dbr.insertItem("Fan", "grad school", dbr.getCategory("Others"));
                dbr.insertItem("Water", "lightning", dbr.getCategory("Others"));
            };

            deleteDB.Click += (object sender, EventArgs e) =>
            {
                DBRepository dbr = new DBRepository();
                dbr.deleteDB();
            };
        }
    }
}