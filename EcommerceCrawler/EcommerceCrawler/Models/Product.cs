using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceCrawler.Models
{
    public class Product
    {
        public int id { get; set; }
        public string name { get; set; }
        public string product_detail_link { get; set; }
        public string review_link { get; set; }
    }
}
