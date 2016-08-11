using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Realms;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

namespace DTG_Ordering_System
{
    class ItemAdapter : BaseAdapter<Item>
    {
		private List<Item> items;
		private Context context;
        private Dictionary<string, int> quantities;

		public ItemAdapter(Context context, List<Item> items, Dictionary<string, int> quantities)
		{
			this.context = context;
			this.items = items;
            this.quantities = quantities;
		}

		public override int Count
		{
			get { return items.Count; }
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override Item this[int position]
		{
			get { return items[position]; }
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View row = convertView;

			if (row == null)
			{
				row = LayoutInflater.From(context).Inflate(Resource.Layout.itemListCustomRow, null, false);
			}

			TextView itemName = row.FindViewById<TextView>(Resource.Id.itemName);
			itemName.Text = items[position].Name;

			TextView itemUnit = row.FindViewById<TextView>(Resource.Id.itemUnit);
			itemUnit.Text = items[position].Unit;

			TextView itemQuantity = row.FindViewById<TextView>(Resource.Id.itemQuantity);
            itemQuantity.Text = quantities[items[position].Id].ToString();

			return row;
		}
    }
}