using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtMuseWeb.Models
{
    public class ResourceBinding
    {
        public Resource Resource { get; set; }
        public MetaData MetaData { get; set; }
        public ResourceData ResourceData { get; set; }
        public Creator Creator { get; set; }
        public Source Source { get; set; }
        public MetaDataCreator MetaDataCreator { get; set; }
        public MetaDataSource GetMetaDataSource { get; set; }
    }
}
