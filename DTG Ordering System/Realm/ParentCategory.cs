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
    class ParentCategory
    {
        public string Name { get; set; }
        public List<Item> Items { get; }

        public ParentCategory(string name, List<Item> items)
        {
            Name = name;
            Items = items;
        }
    }
}