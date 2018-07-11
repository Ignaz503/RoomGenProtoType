using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtMuseWeb.Models
{
    public class Source
    {
        public int ID { get; set; }
        public string Name { get; set; }
        //TODO maybe more fing info
        public ICollection<MetaDataSource> SourceFor { get; set; }
    }
}
