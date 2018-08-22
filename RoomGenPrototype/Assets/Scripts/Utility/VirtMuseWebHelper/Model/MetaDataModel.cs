using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;


namespace VirtMuseWeb.Models
{
    /// <summary>
    /// MetaData of the reousrce gotten from the server
    /// </summary>
    [DataContract]
    public class MetaDataModel
    {
        [DataMember] public int ResourceID { get; set; }

        [DataMember] public string NameOfPiece { get; set; }

        [DataMember] public string DateOfCreation { get; set; }

        [DataMember] public List<Creator> Creators { get; set; }

        [DataMember] public string FurtherInformation { get; set; }

        [DataMember] public List<Source> Sources { get; set; }

        [DataMember] public string License { get; set; }

        public static explicit operator MetaData(MetaDataModel m)
        {
            MetaData meta = new MetaData()
            {
                ResourceID = m.ResourceID.ToString(),
                NameOfPiece = m.NameOfPiece,
                DateOfCreation = m.DateOfCreation,
                License = m.License
            };

            foreach(Creator c in m.Creators)
                meta.Creators.Add(c);

            foreach (Source src in m.Sources)
                meta.Sources.Add(src);

            meta.FurterInformationAccordingToLOD.Add(1, m.FurtherInformation);

            return meta;
        }

    }
}
