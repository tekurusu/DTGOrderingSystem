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
using Java.Lang;
using Android.Graphics;

namespace DTG_Ordering_System
{
    class ExpandableNewOrderAdapter : BaseExpandableListAdapter
    {

        private Context context;
        private List<ParentCategory> headers;
        private Dictionary<string, int> quantities;


        public ExpandableNewOrderAdapter(Activity newContext, List<ParentCategory> headerList, Dictionary<string, int> newQuantities) : base()
        {
            context = newContext;
            headers = headerList;
            quantities = newQuantities;
        }


        public override int GroupCount
        {
            get
            {
                return headers.Count;
            }
        }

        public override bool HasStableIds
        {
            get
            {
                return true;
            }
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }

        public override int GetChildrenCount(int groupPosition)
        {
            return headers[groupPosition].Items.Count;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            var row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(context).Inflate(Resource.Layout.newOrderCustomRow, null);
            }

            //setup childview
            TextView itemName = row.FindViewById<TextView>(Resource.Id.itemName);
            itemName.Text = headers[groupPosition].Items[childPosition].Name;

            TextView itemUnit = row.FindViewById<TextView>(Resource.Id.itemUnit);
            itemUnit.Text = headers[groupPosition].Items[childPosition].Unit;


            TextView itemQuantity = row.FindViewById<TextView>(Resource.Id.itemQuantity);
            itemQuantity.Text = quantities[headers[groupPosition].Items[childPosition].Id].ToString();

            return row;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            throw new NotImplementedException();
        }

        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(context).Inflate(Resource.Layout.newOrderListGroup, null);
            }

            //set header text
            TextView categoryHeader = row.FindViewById<TextView>(Resource.Id.lblListHeader);
            categoryHeader.Text = headers[groupPosition].Name;

            return row;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
    }
}