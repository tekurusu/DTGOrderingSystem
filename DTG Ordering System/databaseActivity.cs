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
                var intent = new Intent(this, typeof(CategoriesActivity));
                StartActivity(intent);
            };

            loadDB.Click += (object sender, EventArgs e) =>
            {
                DBRepository dbr = new DBRepository();
                dbr.CreateDB();
                dbr.CreateTable();

                dbr.insertCategory("Meat Wilbert");

                dbr.insertItem("Chicken", "kilos", dbr.getCategory("Meat Wilbert"));
                dbr.insertItem("Beef", "cows", dbr.getCategory("Meat Wilbert"));
                dbr.insertItem("Pork", "pigs", dbr.getCategory("Meat Wilbert"));
                dbr.insertItem("Justine Young", "manyak", dbr.getCategory("Meat Wilbert"));
                dbr.insertItem("Marty Geronimo", "mats", dbr.getCategory("Meat Wilbert"));
                dbr.insertItem("Jerome Tec", "subjects", dbr.getCategory("Meat Wilbert"));
                dbr.insertItem("Wilbert Uy", "top 1", dbr.getCategory("Meat Wilbert"));
                dbr.insertItem("Zarah Arcega", "grad school", dbr.getCategory("Meat Wilbert"));
                dbr.insertItem("Jules Lui", "lightning", dbr.getCategory("Meat Wilbert"));
            };

            deleteDB.Click += (object sender, EventArgs e) =>
            {
                DBRepository dbr = new DBRepository();
                dbr.deleteDB();
            };

            // Create your application here
        }
    }
}