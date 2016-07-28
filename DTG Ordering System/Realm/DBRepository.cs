using System;
using System.Data;
using System.IO;
using Realms;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace DTG_Ordering_System
{
	public class DBRepository
	{
		private Realm realm;
		private static string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "default.realm");
		RealmConfiguration config = new RealmConfiguration(dbPath, true);

        public void createDB()
		{
			Realm.GetInstance(config);
		}

		public void insertItem(string itemName, string itemUnit, Category category)
		{
			realm = Realm.GetInstance(config);

			using (var transaction = realm.BeginWrite())
			{
				var item = realm.CreateObject<Item>();
				string UUID = Guid.NewGuid().ToString();

				item.Id = UUID;
				item.Name = itemName;
				item.Unit = itemUnit;
				item.Category = category;
				category.Items.Add(item);

				transaction.Commit();
			}
		}

        public string insertCategory(string categoryName)
        {
			realm = Realm.GetInstance(config);
            string UUID;

			using (var transaction = realm.BeginWrite())
			{
				var category = realm.CreateObject<Category>();

				UUID = Guid.NewGuid().ToString();

				category.Id = UUID;
				category.Name = categoryName;

				transaction.Commit();
			}

            return UUID;
		}

        public Category getCategory(string categoryId)
        {
			return Realm.GetInstance(config).All<Category>().Where(c => c.Id == categoryId).First();
        }

        public Item getItem(string itemId)
        {
            return Realm.GetInstance(config).All<Item>().Where(c => c.Id == itemId).First();
        }

		public List<Item> getAllItems(string categoryId)
		{
			List<Item> items = new List<Item>();
			var cat = getCategory(categoryId).Items;

            foreach (var c in cat)
			{
                items.Add(c);
			}

			return items;         
		}

		public List<Category> getAllCategories()
		{
			realm = Realm.GetInstance(config);

			var allCategories = realm.All<Category>().OrderBy(i => i.Name);
			List<Category> categories = new List<Category>();

			foreach (var cat in allCategories)
			{
				categories.Add(cat);
			}
			return categories;
		}

        public void insertOrderedItems(List<Item> items, string orderId, Dictionary<string, int> quantities)
        {
            realm = Realm.GetInstance(config);

            using (var transaction = realm.BeginWrite())
            {
                foreach (Item i in items)
                {
                    var orderedItem = realm.CreateObject<OrderedItem>();
                    orderedItem.Quantity = quantities[i.Id];
                    orderedItem.Item = getItem(i.Id);
                    orderedItem.Order = getOrder(orderId);

                    getItem(i.Id).OrderedItems.Add(orderedItem);
                    getOrder(orderId).OrderedItems.Add(orderedItem);
                }

                transaction.Commit();
            }
        }

        public void updateOrderedItems(List<Item> items, string orderId)
		{
			realm = Realm.GetInstance(config);

			using (var transaction = realm.BeginWrite())
			{
                var orderedItems = getOrder(orderId).OrderedItems;

                foreach (OrderedItem oi in orderedItems)
                {
                    foreach (Item i in items)
                    {

                    }
                }

				foreach (Item i in items)
				{
                    try
                    {
                        //var  = realm.All<OrderedItem>().Where(oi => oi.Order.Id == i.).First();
                        //someOrderedItem.Quantity = i.Quantity;
                        //someOrderedItem.Item = getItem(i.Id);
                        //someOrderedItem.Order = getOrder(orderId);

                        //getItem(i.Id).OrderedItems.Add(someOrderedItem);
                        //getOrder(orderId).OrderedItems.Add(someOrderedItem);
                    }
                    catch
                    {
                        var orderedItem = realm.CreateObject<OrderedItem>();
                        //orderedItem.Quantity = i.Quantity;
                        orderedItem.Item = getItem(i.Id);
                        orderedItem.Order = getOrder(orderId);

                        getItem(i.Id).OrderedItems.Add(orderedItem);
                        getOrder(orderId).OrderedItems.Add(orderedItem);
                    }                    
                }

				transaction.Commit();
			}
		}

		public void updateOrder(string orderId, string deliveryDate)
		{
			realm = Realm.GetInstance(config);

			using (var transaction = realm.BeginWrite())
			{
				var someOrder = realm.All<Order>().Where(o => o.Id == orderId).First();
				someOrder.DeliveryDate = deliveryDate;

				transaction.Commit();
			}
		}

		public string insertOrder(string deliveryDate)
		{
			realm = Realm.GetInstance(config);

			using (var transaction = realm.BeginWrite())
			{
				var order = realm.CreateObject<Order>();
				string UUID = Guid.NewGuid().ToString();

				order.Id = UUID;
				order.DeliveryDate = deliveryDate;
				order.HasSent = false;

				transaction.Commit();

				return order.Id;
			}
		}

		public void sendOrder(string orderId)
		{
			realm = Realm.GetInstance(config);

			using (var transaction = realm.BeginWrite())
			{
				var someOrder = realm.All<Order>().Where(o => o.Id == orderId).First();
				someOrder.HasSent = true;

				transaction.Commit();
			}
		}

		public Order getOrder(string orderId)
		{
			return Realm.GetInstance(config).All<Order>().Where(o => o.Id == orderId).First();
		}

        public List<OrderedItem> getAllOrderedItems(string orderId)
        {
            List<OrderedItem> orderedItems = new List<OrderedItem>();
            var o = getOrder(orderId).OrderedItems;
            foreach (var oi in o)
            {
                orderedItems.Add(oi);
            }
            return orderedItems;
        }

        public List<Order> getAllOrders()
        {
            realm = Realm.GetInstance(config);

			var allOrders = realm.All<Order>();
            List<Order> orders = new List<Order>();

            foreach (Order o in allOrders)
            {
                orders.Add(o);
            }

			return orders;
        }

		public void deleteOrder(string orderId)
		{
			realm = Realm.GetInstance(config);

			using (var transaction = realm.BeginWrite())
			{
				var someOrder = realm.All<Order>().Where(o => o.Id == orderId).First();
				realm.Remove(someOrder);
				transaction.Commit();
			}
		}

        public void deleteDB()
        {
			realm = Realm.GetInstance(config);

			using (var transaction = realm.BeginWrite())
			{
				realm.RemoveAll();
				transaction.Commit();
			}
			realm.Close();
		}

        public void syncDB() //temporary load of database files :))
        {
            deleteDB();
			createDB();

			string ingredients = insertCategory("A. INGREDIENTS");
			string drinks = insertCategory("B. DRINKS & JUICES");
			string dining = insertCategory("C. DINING SUPPLIES");
            string cleaning = insertCategory("D. CLEANING SUPPLIES");
            string meat = insertCategory("E. MEAT");
            string seafoods = insertCategory("F. SEAFOODS");
            string vegetables = insertCategory("G. VEGETABLES");
            string uniforms = insertCategory("I. UNIFORMS");
            string employee = insertCategory("K. EMPLOYEE'S MEAL");
            string rice = insertCategory("L. RICE");
            string office = insertCategory("N. OFFICE SUPPLIES");
            string others = insertCategory("O. OTHERS");

            insertItem("a.01--coconut powder-500g", "kl", getCategory(ingredients));
            insertItem("a.02--Magic sarap 800 grams", "pack", getCategory(ingredients));
            insertItem("a.03--beef cubes", "box", getCategory(ingredients));
            insertItem("a.04--shrimp cubes", "6 pcs per box", getCategory(ingredients));
            insertItem("a.07--cornstarch", "kl", getCategory(ingredients));
            insertItem("a.08--flour", "kl", getCategory(ingredients));
            insertItem("a.09--margarine", "kl", getCategory(ingredients));
            insertItem("a.10--salt", "kl", getCategory(ingredients));
            insertItem("a.11--white sugar", "kl", getCategory(ingredients));
            insertItem("a.12--white pepper", "kl", getCategory(ingredients));
            insertItem("kl", "pack", getCategory(ingredients));
            insertItem("a.13--vetsin", "kl", getCategory(ingredients));
            insertItem("a.14--tamarind powder", "kl", getCategory(ingredients));
            insertItem("a.15--white egg", "tray", getCategory(ingredients));
            insertItem("a.16--fried garlic", "pack", getCategory(ingredients));
            insertItem("a.17--cooked bagoong", "pack", getCategory(ingredients));
            insertItem("a.18--miso", "kl", getCategory(ingredients));
            insertItem("a.19--guinamos 65 grams", "pack", getCategory(ingredients));
            insertItem("a.20--tuna sauce", "gal", getCategory(ingredients));
            insertItem("a.21--Gangnam sauce", "pack", getCategory(ingredients));
            insertItem("a.21--patis", "gal", getCategory(ingredients));
            insertItem("a.22--soy sauce", "cont", getCategory(ingredients));
            insertItem("a.23--vinegar", "cont", getCategory(ingredients));
            insertItem("a.24--cooking oil", "can", getCategory(ingredients));
            insertItem("a.27--batchoy noodles", "kl", getCategory(ingredients));
            insertItem("a.28--chicharon", "pack", getCategory(ingredients));
            insertItem("a.29--chicken oil", "can", getCategory(ingredients));
            insertItem("a.31--marination", "container", getCategory(ingredients));
            insertItem("a.32--achara", "batch", getCategory(ingredients));
            insertItem("a.33--pancit canton", "pack", getCategory(ingredients));
            insertItem("a.34--achuete", "kl", getCategory(ingredients));
            insertItem("a.35--aji vetsin", "kl", getCategory(ingredients));
            insertItem("a.35--batchoy soup mixture", "pack", getCategory(ingredients));
            insertItem("a.37--brown sugar", "kl", getCategory(ingredients));
            insertItem("a.38--catsup", "gal", getCategory(ingredients));
            insertItem("a.39--chinese wine", "bottle", getCategory(ingredients));
            insertItem("a.42--Magic sarap", "pack", getCategory(ingredients));
            insertItem("a.43--carnation condensed", "can", getCategory(ingredients));
            insertItem("a.43--oyster garlic sauce", "bottle", getCategory(ingredients));
            insertItem("a.43--rock salt", "kl", getCategory(ingredients));
            insertItem("a.44--evaporated milk", "can", getCategory(ingredients));
            insertItem("a.44--sesame oil", "gal", getCategory(ingredients));
            insertItem("a.44--sweet chili sauce", "bottle", getCategory(ingredients));
            insertItem("a.45--mais con yelo creme corn", "can", getCategory(ingredients));
            insertItem("a.45--oyster sauce", "galon", getCategory(ingredients));
            insertItem("a.46--today's mixed fruit", "can", getCategory(ingredients));                
            insertItem("a.47--coco vinegar", "cont", getCategory(ingredients));
            insertItem("a.48--mushroom soy sauce", "bottle", getCategory(ingredients));
            insertItem("a.49--rice giniling", "kl", getCategory(ingredients));
            insertItem("a.51--sukang ilocos", "gal", getCategory(ingredients));
            insertItem("a.52--misua", "kl", getCategory(ingredients));
            insertItem("a.53--DM tomato paste", "pack", getCategory(ingredients));
            insertItem("a.54--liver spread", "can", getCategory(ingredients));
            insertItem("a.55--hunts tomato sauce", "kl", getCategory(ingredients));
            insertItem("a.56--lechon sauce", "gal", getCategory(ingredients));
            insertItem("a.57--bagoong isda", "bot", getCategory(ingredients));
            insertItem("a.59--paminta pino", "kl", getCategory(ingredients));
            insertItem("a.60--paminta cracked", "kl", getCategory(ingredients));
            insertItem("a.61--peanut butter", "gal", getCategory(ingredients));
            insertItem("a.62--mayonaise", "gal", getCategory(ingredients));
            insertItem("a.66--nata de coco", "bot", getCategory(ingredients));
            insertItem("a.68--knorr seasoning", "litre", getCategory(ingredients));
            insertItem("a.69--bread crumbs", "kl", getCategory(ingredients));
            insertItem("a.70--baking soda", "kl", getCategory(ingredients));
            insertItem("a.70--ube halaya", "bot", getCategory(ingredients));
            insertItem("a.71--red monggo", "bot", getCategory(ingredients));
            insertItem("a.72--leche flan", "pc", getCategory(ingredients));
            insertItem("a.73--green gulaman powder", "pack", getCategory(ingredients));
            insertItem("a.74--pandan gulaman powder", "pack", getCategory(ingredients));
            insertItem("a.75--alaska all purpose creme", "pack", getCategory(ingredients));
            insertItem("a.76--Corn Flakes", "box", getCategory(ingredients));

            insertItem("b.01--nestea restaurant blend", "box", getCategory(drinks));
            insertItem("b.02--DM pineapple juice", "box", getCategory(drinks));
            insertItem("b.03--robinsons mineral water", "box", getCategory(drinks));

            insertItem("c.00--large plastic for take out", "pack", getCategory(dining));
            insertItem("c.01--aluminum foil x 300 meter", "roll", getCategory(dining));
            insertItem("c.01--Large plastic colored", "pack", getCategory(dining));
            insertItem("c.02--brown supot", "bundle", getCategory(dining));
            insertItem("c.03--cling wrap", "roll", getCategory(dining));
            insertItem("c.04--sauce container", "pc", getCategory(dining));
            insertItem("c.05--16 oz cover", "pc", getCategory(dining));
            insertItem("c.06--8 oz cup", "pc", getCategory(dining));
            insertItem("c.08--16 oz cup with logo DTG", "pcs", getCategory(dining));
            insertItem("c.09--bbq stick BIG", "pack", getCategory(dining));
            insertItem("c.11--8 x 11 plastic", "pack", getCategory(dining));
            insertItem("c.12--tiny plastic", "pack", getCategory(dining));
            insertItem("c.13--medium plastic", "pack", getCategory(dining));
            insertItem("c.14--spoon", "pack", getCategory(dining));
            insertItem("c.15--fork", "pack", getCategory(dining));
            insertItem("c.16--tissue", "kl", getCategory(dining));
            insertItem("c.17--toothpick bamboo", "box", getCategory(dining));
            insertItem("c.18--straw", "kl", getCategory(dining));
            insertItem("c.19--meal tray large for dine in", "pcs", getCategory(dining));
            insertItem("c.20--meal tray small for panga", "pcs", getCategory(dining));
            insertItem("c.22--DTG Rectangular", "pcs", getCategory(dining));
            insertItem("c.23--DTG Oval Plates", "pcs", getCategory(dining));
            insertItem("c.24--DTG Saucer", "pcs", getCategory(dining));
            insertItem("c.25--DTG Serving Bowl", "pcs", getCategory(dining));
            insertItem("c.26--DTG Small Soup Bowl", "pcs", getCategory(dining));
            insertItem("c.29--Green big bowl for display", "pcs", getCategory(dining));
            insertItem("c.30--Green bowl belly", "pcs", getCategory(dining));
            insertItem("c.31--Green rectangular plate", "pcs", getCategory(dining));
            insertItem("c.33--DTG bangus belly bowl", "pcs", getCategory(dining));
            insertItem("c.34--Green Soup Bowl", "pcs", getCategory(dining));
            insertItem("c.35--Rice Cups", "pcs", getCategory(dining));
            insertItem("c.36--Paper bowl w/ cover", "pcs", getCategory(dining));
            insertItem("c.37-Uling", "sack", getCategory(dining));
            insertItem("c.38--Trash bag black xl", "pack", getCategory(dining));
            insertItem("c.40--rectangular box w/ cover", "pcs", getCategory(dining));
            insertItem("c.41--Meal box", "pc", getCategory(dining));
            insertItem("c.45--brown supot big", "pack", getCategory(dining));
            insertItem("c.46--12 x 18 clear", "pack", getCategory(dining));
            insertItem("c.46--brown supot medium", "pack", getCategory(dining));
            insertItem("c.47--12 x 18 colored", "pack", getCategory(dining));
            insertItem("c.47--XL stripe plastic", "pack", getCategory(dining));
            insertItem("c.48--2 x 4 pp plastic", "pack", getCategory(dining));
            insertItem("c.48--paper cups 8 oz", "pc", getCategory(dining));
            insertItem("c.49--32 x 36 pp plastic", "pack", getCategory(dining));
            insertItem("c.49--paper cups 16 oz plain", "pc", getCategory(dining));
            insertItem("c.49--paper plate", "pc", getCategory(dining));
            insertItem("c.50--paper plate for side dish", "pc", getCategory(dining));
            insertItem("c.52--disposable gloves", "pack", getCategory(dining));
            insertItem("c.55--10 x 14 plain", "pack", getCategory(dining));
            insertItem("c.55--wax paper for rice take out", "pc", getCategory(dining));
            insertItem("c.56--biodegradable spoon", "pc", getCategory(dining));
            insertItem("c.57--biodegradable fork", "pc", getCategory(dining));
            insertItem("c.58--12 oz cup", "pcs", getCategory(dining));
            insertItem("c.58--paper bowl for sigang", "pc", getCategory(dining));
            insertItem("c.59--paper bowl for side dish", "pc", getCategory(dining));
            insertItem("c.60--Bilao", "pc", getCategory(dining));
            insertItem("c.62--3 division plates", "pcs.", getCategory(dining));
            insertItem("c.64--soup bowl for sinanglaw", "pcs", getCategory(dining));
            insertItem("c.65--bagnet boy soup bowl", "pcs", getCategory(dining));

            insertItem("d.01--clorox", "gal", getCategory(cleaning));
            insertItem("d.02--dishwashing liquid", "gal", getCategory(cleaning));
            insertItem("d.03--detergent soap", "kl", getCategory(cleaning));
            insertItem("d.05--scotchbrite", "pc", getCategory(cleaning));
            insertItem("d.06--foam sponge", "pc", getCategory(cleaning));
            insertItem("d.07--zest soap", "pc", getCategory(cleaning));
            insertItem("d.08--alcohol", "bot", getCategory(cleaning));
            insertItem("d.09--towel", "pc", getCategory(cleaning));
            insertItem("d.10--pot holder", "pc", getCategory(cleaning));
            insertItem("d.11--scrub pad", "pc", getCategory(cleaning));
            insertItem("d.12--sanisoft ( hand soap )", "gal", getCategory(cleaning));
            insertItem("d.14--degreaser", "gal", getCategory(cleaning));
            insertItem("d.18--blue towel", "pc", getCategory(cleaning));
            insertItem("d.18--green towel", "pc", getCategory(cleaning));
            insertItem("d.19--pink towel", "pc", getCategory(cleaning));

            insertItem("e.01--pork sisig", "pc", getCategory(meat));
            insertItem("e.05--porkchop marinated", "kl", getCategory(meat));
            insertItem("e.06--liempo marinated", "kl", getCategory(meat));
            insertItem("e.08--inasal chicken", "whole", getCategory(meat));
            insertItem("e.09--Pork BBQ 70g", "stick", getCategory(meat));
            insertItem("e.10--Chicken BBQ 70g", "stick", getCategory(meat));
            insertItem("e.12--Chicken BBQ 50g", "stick", getCategory(meat));
            insertItem("e.15--lumpia sisig", "pc", getCategory(meat));
            insertItem("e.17--Beef Laman", "kl", getCategory(meat));
            insertItem("e.17--carabeef", "kl", getCategory(meat));
            insertItem("e.29--molo", "pack", getCategory(meat));
            insertItem("e.32--Longganisa", "kl", getCategory(meat));
            insertItem("e.38--pork ribs", "kl", getCategory(meat));
            insertItem("e.42--pigue", "kl", getCategory(meat));
            insertItem("e.43--pork liver", "kl", getCategory(meat));
            insertItem("e.44--utak", "pc", getCategory(meat));
            insertItem("e.45--boneless ulo", "kl", getCategory(meat));
            insertItem("e.49--chicken liver", "kilo", getCategory(meat));
            insertItem("e.50--bagnet", "kl", getCategory(meat));
            insertItem("e.51--beef bones", "kl", getCategory(meat));
            insertItem("e.53--beef kamto", "kl", getCategory(meat));
            insertItem("e.55--bopis", "kl", getCategory(meat));
            insertItem("e.56--goto", "kl", getCategory(meat));
            insertItem("e.57--papaitan", "kl", getCategory(meat));
            insertItem("e.58--puso ng baboy", "kl", getCategory(meat));
            insertItem("e.60--chicken plain", "kl", getCategory(meat));
            insertItem("e.63--kasim pork", "kl", getCategory(meat));

            insertItem("f.01--tuna lumpia", "pc", getCategory(seafoods));
            insertItem("f.02--tanigue ( kilaw use )", "kl", getCategory(seafoods));
            insertItem("f.03--tilapia", "kl", getCategory(seafoods));
            insertItem("f.04--bangus regular", "pc", getCategory(seafoods));
            insertItem("f.05--bangus belly", "pack", getCategory(seafoods));
            insertItem("f.06--blue marlin", "pc", getCategory(seafoods));
            insertItem("f.07--big squid illex x 10 kgs", "box", getCategory(seafoods));
            insertItem("f.08--tuna belly", "pc", getCategory(seafoods));
            insertItem("f.09--tuna panga grill", "pc", getCategory(seafoods));
            insertItem("f.10--tuna panga sigang", "kl", getCategory(seafoods));
            insertItem("f.11--maya head reg", "pc", getCategory(seafoods));
            insertItem("f.13--salmon head small", "pc", getCategory(seafoods));
            insertItem("f.14--salmon head medium", "pc", getCategory(seafoods));
            insertItem("f.15--salmon head large", "pc", getCategory(seafoods));
            insertItem("f.16--tuna tail medium", "pc", getCategory(seafoods));
            insertItem("f.17--tuna tail large", "pc", getCategory(seafoods));
            insertItem("f.18--tuna tail XL", "pc", getCategory(seafoods));
            insertItem("f.19--bangus marinated", "pc", getCategory(seafoods));
            insertItem("f.20--hito", "pc", getCategory(seafoods));
            insertItem("f.21--tuna sisig", "order", getCategory(seafoods));
            insertItem("f.22--squid sisig", "pc", getCategory(seafoods));
            insertItem("f.22--tuna trimmings", "kilo", getCategory(seafoods));
            insertItem("f.23--fish fillet plain", "pc", getCategory(seafoods));
            insertItem("f.23--giant squid", "kilo", getCategory(seafoods));
            insertItem("f.24--fish fillet plain", "pc", getCategory(seafoods));
            insertItem("f.24--fresh shrimp", "kl", getCategory(seafoods));
            insertItem("f.25--Calamares 100g", "pc", getCategory(seafoods));
            insertItem("f.26--shrimp dried", "kl", getCategory(seafoods));
            insertItem("f.26--Tuna dynamite", "pc", getCategory(seafoods));
            insertItem("f.29--lolligo squid small", "box", getCategory(seafoods));
            insertItem("f.31--salmon belly 100g", "pc", getCategory(seafoods));

            insertItem("g. alamang", "kl", getCategory(vegetables));
            insertItem("g. ampalaya", "kl", getCategory(vegetables));
            insertItem("g. bawang", "kl", getCategory(vegetables));
            insertItem("g. bellpepper", "kl", getCategory(vegetables));
            insertItem("g. bulaklak kalabasa", "kl", getCategory(vegetables));
            insertItem("g. calamansi", "kl", getCategory(vegetables));
            insertItem("g. carrots", "kl", getCategory(vegetables));
            insertItem("g. dahon ampalaya", "kl", getCategory(vegetables));
            insertItem("g. dahon laurel", "kl", getCategory(vegetables));
            insertItem("g. dahon saging", "kl", getCategory(vegetables));
            insertItem("g. dahon sibuyas", "kl", getCategory(vegetables));
            insertItem("g. dried alamang", "kl", getCategory(vegetables));
            insertItem("g. food color violet", "bottle", getCategory(vegetables));
            insertItem("g. food color yellow-orange", "bottle", getCategory(vegetables));
            insertItem("g. gabi", "kl", getCategory(vegetables));
            insertItem("g. galapong", "kl", getCategory(vegetables));
            insertItem("g. green peas", "kl", getCategory(vegetables));
            insertItem("g. kamatis", "kl", getCategory(vegetables));
            insertItem("g. kamias", "kl", getCategory(vegetables));
            insertItem("g. kamote", "kl", getCategory(vegetables));
            insertItem("g. kangkong", "bl", getCategory(vegetables));
            insertItem("g. langka", "kl", getCategory(vegetables));
            insertItem("g. luya", "kl", getCategory(vegetables));
            insertItem("g. malagkit", "kl", getCategory(vegetables));
            insertItem("g. okra", "kl", getCategory(vegetables));
            insertItem("g. papaya buo", "kl", getCategory(vegetables));
            insertItem("g. papaya gadgad", "kl", getCategory(vegetables));
            insertItem("g. patatas", "kl", getCategory(vegetables));
            insertItem("g. patola", "kl", getCategory(vegetables));
            insertItem("g. pechay tagalog", "kl", getCategory(vegetables));
            insertItem("g. pipino", "kl", getCategory(vegetables));
            insertItem("g. puso ng saging buo", "kl", getCategory(vegetables));
            insertItem("g. puso ng saging gayat", "kl", getCategory(vegetables));
            insertItem("g. red onion", "kl", getCategory(vegetables));
            insertItem("g. repolyo", "kl", getCategory(vegetables));
            insertItem("g. saba", "pc", getCategory(vegetables));
            insertItem("g. sago", "kl", getCategory(vegetables));
            insertItem("g. saluyot", "kl", getCategory(vegetables));
            insertItem("g. sigarilyas", "kl", getCategory(vegetables));
            insertItem("g. sili labuyo", "kl", getCategory(vegetables));
            insertItem("g. sili long", "kl", getCategory(vegetables));
            insertItem("g. singkamas", "kl", getCategory(vegetables));
            insertItem("g. sitaw", "kl", getCategory(vegetables));
            insertItem("g. talong", "kl", getCategory(vegetables));
            insertItem("g. ube", "kl", getCategory(vegetables));
            insertItem("g.01--baguio beans", "kl", getCategory(vegetables));
            insertItem("g.02--okra", "kl", getCategory(vegetables));
            insertItem("g.03--cabbage", "kl", getCategory(vegetables));
            insertItem("g.04--calamansi", "kl", getCategory(vegetables));
            insertItem("g.05--carrots", "kl", getCategory(vegetables));
            insertItem("g.05--dahon ampalaya", "kl", getCategory(vegetables));
            insertItem("g.06--sayote", "kl", getCategory(vegetables));
            insertItem("g.07--ampalaya", "kl", getCategory(vegetables));
            insertItem("g.08--eggplant", "kl", getCategory(vegetables));
            insertItem("g.09--garlic hubad", "kl", getCategory(vegetables));
            insertItem("g.1-ampalaya", "kl", getCategory(vegetables));
            insertItem("g.10--ginger", "kl", getCategory(vegetables));
            insertItem("g.10-celery", "kl", getCategory(vegetables));
            insertItem("g.11-- kamias", "kl", getCategory(vegetables));
            insertItem("g.11--kamatis", "kl", getCategory(vegetables));
            insertItem("g.11-dahon ampalaya", "kl", getCategory(vegetables));
            insertItem("g.12--kangkong", "bl", getCategory(vegetables));
            insertItem("g.12-dahon laurel", "kl", getCategory(vegetables));
            insertItem("g.13--labuyo", "kl", getCategory(vegetables));
            insertItem("g.13-dahon saging bundle", "bl", getCategory(vegetables));
            insertItem("g.14-dahon saging tupi", "kl", getCategory(vegetables));
            insertItem("g.15--manggang hilaw", "kl", getCategory(vegetables));
            insertItem("g.15-dahon sibuyas", "kl", getCategory(vegetables));
            insertItem("g.16--togue", "kl", getCategory(vegetables));
            insertItem("g.16-dried alamang", "kl", getCategory(vegetables));
            insertItem("g.17--mustasa", "kl", getCategory(vegetables));
            insertItem("g.17-food color violet", "kl", getCategory(vegetables));
            insertItem("g.18--white onion", "kl", getCategory(vegetables));
            insertItem("g.18-food color yellow-orange", "kl", getCategory(vegetables));
            insertItem("g.19--monggo", "kl", getCategory(vegetables));
            insertItem("g.19-gabi", "kl", getCategory(vegetables));
            insertItem("g.2-bagoong hilaw", "kl", getCategory(vegetables));
            insertItem("g.20--bell pepper", "kl", getCategory(vegetables));
            insertItem("g.20--bulaklak kalabasa", "kl", getCategory(vegetables));
            insertItem("g.20-galapong", "kl", getCategory(vegetables));
            insertItem("g.21--pipino", "kl", getCategory(vegetables));
            insertItem("g.21-green peas", "kl", getCategory(vegetables));
            insertItem("g.22--sili long", "kl", getCategory(vegetables));
            insertItem("g.22-kalabasa whole", "kl", getCategory(vegetables));
            insertItem("g.23--sitaw bundle", "bundle", getCategory(vegetables));
            insertItem("g.23-kamatis", "kl", getCategory(vegetables));
            insertItem("g.24--Dahon ng saging tupi", "kl", getCategory(vegetables));
            insertItem("g.24--Dahon saging tupi", "kl", getCategory(vegetables));
            insertItem("g.24-kamias", "kl", getCategory(vegetables));
            insertItem("g.25-kamote", "kl", getCategory(vegetables));
            insertItem("g.26--kalabasa whole", "pc", getCategory(vegetables));
            insertItem("g.26-kangkong tali", "kl", getCategory(vegetables));
            insertItem("g.27--pechay tagalog", "kl", getCategory(vegetables));
            insertItem("g.27-kangkong bundle", "bl", getCategory(vegetables));
            insertItem("g.28--bagoong hilaw", "kl", getCategory(vegetables));
            insertItem("g.28--green peas", "kl", getCategory(vegetables));
            insertItem("g.28--spring onion", "kl", getCategory(vegetables));
            insertItem("g.28-langka", "kl", getCategory(vegetables));
            insertItem("g.29--lumpia wrapper", "pc", getCategory(vegetables));
            insertItem("g.29-lumpia wrapper medium", "pc", getCategory(vegetables));
            insertItem("g.3-baguio beans", "kl", getCategory(vegetables));
            insertItem("g.30--TOTAL VEGETABLES", "bulk", getCategory(vegetables));
            insertItem("g.30-lumpia wrapper xl", "pc", getCategory(vegetables));
            insertItem("g.31--singkamas", "kl", getCategory(vegetables));
            insertItem("g.31-luya", "kl", getCategory(vegetables));
            insertItem("g.32-malagkit", "kl", getCategory(vegetables));
            insertItem("g.33--celery", "kl", getCategory(vegetables));
            insertItem("g.33-monggo", "kl", getCategory(vegetables));
            insertItem("g.34-okra", "kl", getCategory(vegetables));
            insertItem("g.35-onion leeks", "kl", getCategory(vegetables));
            insertItem("g.36-papaya buo", "kl", getCategory(vegetables));
            insertItem("g.37-papaya gadgad", "kl", getCategory(vegetables));
            insertItem("g.38--bagoong hilaw", "kl", getCategory(vegetables));
            insertItem("g.38-patatas", "kl", getCategory(vegetables));
            insertItem("g.39--papaya gadgad", "kl", getCategory(vegetables));
            insertItem("g.39-patola", "kl", getCategory(vegetables));
            insertItem("g.4-bawang hubad", "kl", getCategory(vegetables));
            insertItem("g.40--TOTAL VEGETABLES", "bulk", getCategory(vegetables));
            insertItem("g.40-pechay tagalog", "kl", getCategory(vegetables));
            insertItem("g.41--bawang w/ balat", "kl", getCategory(vegetables));
            insertItem("g.41-pipino", "kl", getCategory(vegetables));
            insertItem("g.42--lumpia wrapper Xlarge", "kl", getCategory(vegetables));
            insertItem("g.42-puso ng saging buo", "kl", getCategory(vegetables));
            insertItem("g.43--Lumpia wrapper medium", "kl", getCategory(vegetables));
            insertItem("g.43-puso ng saging gayat", "kl", getCategory(vegetables));
            insertItem("g.44--onion leeks", "kl", getCategory(vegetables));
            insertItem("g.44-red onion", "pc", getCategory(vegetables));
            insertItem("g.45--papaya buo", "kl", getCategory(vegetables));
            insertItem("g.45-repolyo", "kl", getCategory(vegetables));
            insertItem("g.46-saba", "pc", getCategory(vegetables));
            insertItem("g.47-sago", "kl", getCategory(vegetables));
            insertItem("g.48-saluyot", "bl", getCategory(vegetables));
            insertItem("g.49--puso ng saging gayat", "kl", getCategory(vegetables));
            insertItem("g.49-sayote", "kl", getCategory(vegetables));
            insertItem("g.5-bawang with balat", "kl", getCategory(vegetables));
            insertItem("g.50--camote", "kl", getCategory(vegetables));
            insertItem("g.50-sigarilyas", "kl", getCategory(vegetables));
            insertItem("g.51--langka", "kl", getCategory(vegetables));
            insertItem("g.51--Saging Saba", "pc", getCategory(vegetables));
            insertItem("g.51--ube", "kl", getCategory(vegetables));
            insertItem("g.51-sili labuyo", "kl", getCategory(vegetables));
            insertItem("g.52--patatas", "kl", getCategory(vegetables));
            insertItem("g.52-sili long", "kl", getCategory(vegetables));
            insertItem("g.52patatas", "kl", getCategory(vegetables));
            insertItem("g.53--patola", "kl", getCategory(vegetables));
            insertItem("g.53-singkamas", "kl", getCategory(vegetables));
            insertItem("g.54--puso ng saging buo", "kl", getCategory(vegetables));
            insertItem("g.54--sitaw bundle", "bl", getCategory(vegetables));
            insertItem("g.55--red onion", "kl", getCategory(vegetables));
            insertItem("g.55--saluyot", "kl", getCategory(vegetables));
            insertItem("g.56--galapong", "kl", getCategory(vegetables));
            insertItem("g.56--sigarilyas", "kl", getCategory(vegetables));
            insertItem("g.56-talong", "kl", getCategory(vegetables));
            insertItem("g.57--gabi", "kl", getCategory(vegetables));
            insertItem("g.57--sago", "kl", getCategory(vegetables));
            insertItem("g.57-togue", "kl", getCategory(vegetables));
            insertItem("g.58-ube", "kl", getCategory(vegetables));
            insertItem("g.59--dried alamang", "kl", getCategory(vegetables));
            insertItem("g.59-Pandan tali", "tali", getCategory(vegetables));
            insertItem("g.6-bellpeper", "kl", getCategory(vegetables));
            insertItem("g.6-bellpepper", "kl", getCategory(vegetables));
            insertItem("g.60--food color violet", "bottle", getCategory(vegetables));
            insertItem("g.60-white onion", "kl", getCategory(vegetables));
            insertItem("g.61-TOTAL VEGETABLES", "bulk", getCategory(vegetables));
            insertItem("g.62-mustasa", "kl", getCategory(vegetables));
            insertItem("g.63-mangga hilaw", "kl", getCategory(vegetables));
            insertItem("g.64--rice giniling", "kl", getCategory(vegetables));
            insertItem("g.64-Monggo crack", "kl", getCategory(vegetables));
            insertItem("g.7-bulaklak kalabasa", "kl", getCategory(vegetables));
            insertItem("g.8-calamansi", "kl", getCategory(vegetables));
            insertItem("g.9-carrots", "kl", getCategory(vegetables));
            insertItem("g.BUNDLE dahon ng saging", "bundle", getCategory(vegetables));
            insertItem("g.monggo", "kl", getCategory(vegetables));

            insertItem("i.03--full apron black", "pc", getCategory(uniforms));

            insertItem("k.00--Corn beef", "can", getCategory(employee));
            insertItem("k.01--Luncheon Meat", "can", getCategory(employee));
            insertItem("k.02--Sardines Big", "can", getCategory(employee));
            insertItem("k.03--Century Tuna Big", "can", getCategory(employee));
            insertItem("k.04--Misua", "pack", getCategory(employee));
            insertItem("k.05--Sotanghon", "pack", getCategory(employee));
            insertItem("k.06--coffee", "pack", getCategory(employee));
            insertItem("k.07--creamer", "pack", getCategory(employee));
            insertItem("k.08--porkchop x 12", "kl", getCategory(employee));
            insertItem("k.09--Chicken Plain", "kl", getCategory(employee));
            insertItem("k.10--Tilapia x 6 pieces", "kl", getCategory(employee));
            insertItem("k.11--galunggong", "kl", getCategory(employee));
            insertItem("k.12--Marlin scrap", "kl", getCategory(employee));
            insertItem("k.13--Salmon Head undersize", "kl", getCategory(employee));
            insertItem("k.14--fish fillet scrap", "kl", getCategory(employee));
            insertItem("k.15--pigue", "kl", getCategory(employee));
            insertItem("k.16--Tuna buntot scrap", "kl", getCategory(employee));
            insertItem("k.17--salmon belly scrap", "kl", getCategory(employee));
            insertItem("k.17--Vege for employees", "lot", getCategory(employee));
            insertItem("k.18--pork gininling", "kl", getCategory(employee));
            insertItem("k.18--salmon belly scrap", "kl", getCategory(employee));
            insertItem("k.20--cara beef", "kl", getCategory(employee));
            insertItem("k.21--pork ribs", "kl", getCategory(employee));
            insertItem("k.24--Pusit illex scrap", "kl", getCategory(employee));

            insertItem("l.01--rice", "sack", getCategory(rice));

            insertItem("n.02--mimeo paper", "ream", getCategory(office));
            insertItem("n.03--bond paper x 10", "pack", getCategory(office));
            insertItem("n.04--correction tape", "pc", getCategory(office));
            insertItem("n.05--scotch tape", "pc", getCategory(office));
            insertItem("n.06--ballpen", "pc", getCategory(office));
            insertItem("n.07--petty cash voucher", "pack", getCategory(office));
            insertItem("n.08--carbon paper x 5", "pack", getCategory(office));
            insertItem("n.09--staple wire", "box", getCategory(office));
            insertItem("n.10--paper fastener", "pack", getCategory(office));
            insertItem("n.13--white board marker", "pc", getCategory(office));
            insertItem("n.14--Thermal paper", "pc", getCategory(office));
            insertItem("n.16--cashier's cash form x 20", "pack", getCategory(office));
            insertItem("n.17--Davao tuna ATD form x 20", "pack", getCategory(office));
            insertItem("n.20--DTG reimbursement form", "pack", getCategory(office));
            insertItem("n.22--DTG Attendance summary form", "pack", getCategory(office));
            insertItem("n.24--DTG Work schedule form", "pack", getCategory(office));
            insertItem("n.29--BB Attendance summary form", "pack", getCategory(office));

            insertItem("o.01--electrical materials", "lot", getCategory(others));
            insertItem("o.02--kitchenwares", "lot", getCategory(others));
            insertItem("o.04--uniforms", "lot", getCategory(others));
            insertItem("o.05--face mask", "pc", getCategory(others));
            insertItem("o.08--stimulant", "pc", getCategory(others));
            insertItem("o.10--band aid", "pcs", getCategory(others));
            insertItem("o.10--vaporub", "pc", getCategory(others));
            insertItem("o.21--Lason sa ipis", "pck", getCategory(others));
        }
	}
}

