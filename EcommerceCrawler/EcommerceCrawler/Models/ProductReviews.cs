using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceCrawler.Models
{
    public class ProductReviews
    {
        public int id { get; set; }
        public int product_id { get; set; }
        public string customer { get; set; }
        public string body { get; set; }
        public int stars { get; set; }
    }
}
