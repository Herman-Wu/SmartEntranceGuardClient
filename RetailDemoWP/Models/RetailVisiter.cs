using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailDemoWP.Models
{
    public class DoorVisiter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string CurrentRegion { get; set; }
        public bool IsAdult { get; set; }
        public bool IsRacy  { get; set; }
        public List<string> Tags { get; set; }
    }
}
