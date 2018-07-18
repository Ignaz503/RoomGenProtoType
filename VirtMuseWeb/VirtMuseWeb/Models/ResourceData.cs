using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtMuseWeb.Models
{

    public class ResourceData
    {
        public int ID { get; set; }
        public byte[] Data { get; set; }

        public int ResourceID { get; set; }
        public Resource Resource { get; set; }
    }
}
