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

namespace DTG_Ordering_System
{
    class ExpandableNewOrderAdapter : BaseExpandableListAdapter
    {

        private Context context;
        //private List<string> headers;
        //private List<string> subItems;
        private List<ParentCategory> headers;
        private List<Item> subItems;


        public ExpandableNewOrderAdapter(Activity newContext, List<ParentCategory> headerList) : base()
        {
            context = newContext;
            headers = headerList;
            //subItems = subItemList;
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
            //itemName.Text = subItems[childPosition].Name;
            itemName.Text = headers[groupPosition].Items[childPosition].Name;

            TextView itemUnit = row.FindViewById<TextView>(Resource.Id.itemUnit);
            //itemUnit.Text = subItems[childPosition].Unit;
            itemUnit.Text = headers[groupPosition].Items[childPosition].Unit;


            TextView itemQuantity = row.FindViewById<TextView>(Resource.Id.itemQuantity);
            //itemQuantity.Text = subItems[childPosition].Quantity.ToString();
            itemQuantity.Text = headers[groupPosition].Items[childPosition].Quantity.ToString();

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