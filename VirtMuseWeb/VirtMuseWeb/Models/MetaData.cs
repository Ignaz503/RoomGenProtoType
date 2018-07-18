using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VirtMuseWeb.Models
{
    public class MetaData
    { 
        public int ID { get; set; }
        public string NameOfPiece { get; set; }

        [Required]
        public ICollection<MetaDataCreator> Creators { get; set; }

        [Display(Name ="Date of Creation")]
        public DateTime DateOfCreation { get; set; }
        public string FurtherInformation { get; set; }

        [Required]
        public ICollection<MetaDataSource> Sources { get; set; }

        public string License { get; set; }

        [Required]
        public Resource Resource { get; set; }
    }
}
