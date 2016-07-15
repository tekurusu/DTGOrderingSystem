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
		List<Category> categories = new List<Category>();
		List<String> categoryName = new List<String>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.categoryList);

			DBRepository dbr = new DBRepository();

			categories = dbr.GetAllCategories();

			foreach (Category c in categories)
			{
				categoryName.Add(c.Name);
			}

            ListView categoryList = FindViewById<ListView>(Resource.Id.categoriesListView);
			ArrayAdapter categoryAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, categoryName);
            categoryList.Adapter = categoryAdapter;

            categoryList.ItemClick += CategoryList_ItemClick;
            
        }

        private void CategoryList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var intent = new Intent(this, typeof(ItemsActivity));
			var cat = categories[e.Position];
			intent.PutExtra("CategoryID", cat.Id);
            StartActivity(intent);
        }
    }
}