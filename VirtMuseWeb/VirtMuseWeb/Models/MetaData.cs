using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtMuseWeb.Models
{
    public class MetaData
    { 
        public int ID { get; set; }
        public string NameOfPiece { get; set; }
        public ICollection<MetaDataCreator> Creators { get; set; }
        public DateTime DateOfCreation { get; set; }
        public string FurtherInformation { get; set; }
        public ICollection<MetaDataSource> Sources { get; set; }
        public string License { get; set; }

        public int ResourceID { get; set; }
        public Resource Resource { get; set; }
    }
}
