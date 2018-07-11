using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtMuseWeb.Models
{
    public enum ResourceType
    {
        Image = 1,
        Mesh = 2
    }

    public class Resource
    {
        public int ID { get; set; }
        public ResourceType ResourceType { get; set; }

        public MetaData MetaData { get; set; }
        public string InteractionBehaviour { get; set; }
        //TODO currently just IP make actuall user login
        //used to find when checking what metadata is associated
        //and post time used to find newest
        public string User { get; set; }
        public DateTime PostTime { get; set; }

        public ICollection<ResourceData> ResourceData;
    }
}
