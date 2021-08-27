using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceCrawler.Dtos
{
    public class ProductDto
    {
        public string name { get; set; }
        public float discount { get; set; }
        public double totalPrice { get; set; }
        public string rating { get; set; }
        public Href href { get; set; } = new Href();
    }
}
