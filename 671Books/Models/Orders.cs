using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _671Books.Models
{
    public class Orders
    {
        public int OrderID { get; set; }
        public int ID { get; set; }
        public string OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public int Quantity { get; set; }
        public string ShippingAddress { get; set; }
        public string name { get; set; }
        public string author { get; set; }
        public string publisher { get; set; }
        public string genre { get; set; }
        public int year { get; set; }
        public int edition { get; set; }
        public decimal price { get; set; }
        public string Status {get; set;}
    }
}