using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailDemoWP.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string PName { get; set; }
        public string Description { get; set; }
        public string RegionName { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public int Count { get; set; }
        public bool IsOnSale { get; set; }
        public string PGender { get; set; }
        public string ProducedDay { get; set; }
        public string InFactoryDay { get; set; }
        public List<string> Tags { get; set; }
    }
}
