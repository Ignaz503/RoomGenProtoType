using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtMuseWeb.Models
{

    public class MetaDataCreator
    {
        //public int ID { get; set; }
        public int MetaDataID { get; set; }
        public MetaData MetaData { get; set; }
        public int CreatorID { get; set; }
        public Creator Creator { get; set; }
    }
}
