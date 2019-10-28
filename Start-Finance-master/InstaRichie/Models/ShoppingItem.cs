using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartFinance.Models
{
    class ShoppingItem
    {
        [PrimaryKey, AutoIncrement]
        public int ShoppingItemID { get; set; }

        [NotNull]
        public string ShopName { get; set; }

        [Unique, NotNull]
        public string NameOfItem { get; set; }

        [NotNull]
        public DateTime ShoppingDate { get; set; }

        [NotNull]
        public double PriceQuoted { get; set; }
    }
}
