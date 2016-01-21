using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailDemoWP.Models
{
    public class RetailMember
    {
        private string _userName;

        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public List<string> Tags { get; set; }
    }
}

