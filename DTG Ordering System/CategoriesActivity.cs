using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Mono;

namespace DTG_Ordering_System
{
    [Activity(Label = "Categories Screen", Icon = "@drawable/icon")]
    public class CategoriesActivity : Activity
    {
        List<string> categories;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //DBRepository dbr = new DBRepository();
            //dbr.CreateDB();
            //dbr.CreateTable();

            // Create your application here
            SetContentView(Resource.Layout.categoryList);

            categories = new List<string>();

            categories.Add("Meat");
            categories.Add("Spices");
            categories.Add("Others");

            ListView categoryList = FindViewById<ListView>(Resource.Id.categoriesListView);
            ArrayAdapter categoryAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, categories);
            categoryList.Adapter = categoryAdapter;

            categoryList.ItemClick += CategoryList_ItemClick;
            
        }

        private void CategoryList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var intent = new Intent(this, typeof(ItemsActivity));
            intent.PutExtra("CategoryID", e.Position);
            StartActivity(intent);
        }
    }
}