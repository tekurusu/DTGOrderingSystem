using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Mono;
using Android.Preferences;

namespace DTG_Ordering_System
{
    [Activity(Label = "Choose a Category:", Icon = "@drawable/icon")]
    public class CategoriesActivity : Activity
    {
		List<Category> categories = new List<Category>();
		List<String> categoryName = new List<String>();
        private ImageButton backButton3;
        private TextView logout;
        private CategoryAdapter adapter;
        private string branchId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.categoryList);

			DBRepository dbr = new DBRepository();

			categories = dbr.getAllCategories();

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            branchId = prefs.GetString("branchId", null);

            foreach (Category c in categories)
			{
				categoryName.Add(c.Name);
			}

            ListView categoryList = FindViewById<ListView>(Resource.Id.categoriesListView);
            TextView branchName = FindViewById<TextView>(Resource.Id.branchName);
            branchName.Text = dbr.getBranchName(branchId);
            backButton3 = FindViewById<ImageButton>(Resource.Id.backButton3);
            logout = FindViewById<TextView>(Resource.Id.logout);
            logout.Visibility = ViewStates.Invisible;

            adapter = new CategoryAdapter(this, categoryName, this);
            categoryList.Adapter = adapter;

            categoryList.ItemClick += CategoryList_ItemClick;
            backButton3.Click += BackButton3_Click;
        }

        private void BackButton3_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        private void CategoryList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var intent = new Intent(this, typeof(ItemsActivity));
			var cat = categories[e.Position];
			intent.PutExtra("categoryId", cat.Id);
            intent.PutExtra("categoryName", cat.Name);
            StartActivityForResult(intent,0);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            SetResult(Result.Ok, data);
			if (data != null)
            	Finish();
        }
    }
}