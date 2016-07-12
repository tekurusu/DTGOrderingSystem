using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace DTG_Ordering_System
{
    [Table("Item")]

    public class Item
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        //[ForeignKey(typeof(Category))]
        //public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }

        //[ManyToOne]
        //public Category Category { get; set; }
    }
}