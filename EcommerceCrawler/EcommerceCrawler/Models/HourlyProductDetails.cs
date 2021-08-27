using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceCrawler.Models
{
    public class HourlyProductDetails
    {
        public int id { get; set; }
        public string name { get; set; }
        public int batch_id { get; set; }
        public DateTime fetch_time { get; set; } = DateTime.Now;
        public string rating { get; set; }
        public string product_detail_link { get; set; }
        public double discount { get; set; }
        public double total_price { get; set; }
    }
}
