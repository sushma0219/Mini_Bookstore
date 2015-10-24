using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _671Books.Models
{
    public class Book
    {
        public int ID { get; set; }
        public string Author {get; set;}
        public string Genre { get; set; }
        public int Year { get; set; }
        public string Name { get; set;}
        public string Publisher { get; set; }
        public decimal Price { get; set; }
        public string ISBN { get; set; }
        public string BookCover { get; set; }
        public int Edition { get; set; }
        public int QuantityInStore { get; set; }
        public int QuantitySold { get; set; }
    }
}