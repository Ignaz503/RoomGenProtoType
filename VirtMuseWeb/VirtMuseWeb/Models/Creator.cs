using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VirtMuseWeb.Models
{

    public class Creator
    {
        public int ID { get; set; }
        public string Name { get; set; }
        [Display(Name = "Birthday")]
        public DateTime BirthDate { get; set; }
        [Display(Name = "Day of Death")]
        [DisplayFormat(NullDisplayText = "...")]
        public DateTime? DateOfDeath { get; set; }
        public ICollection<MetaDataCreator> PiecesCreated { get; set; }
    }
}
