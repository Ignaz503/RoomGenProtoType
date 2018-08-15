using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace VirtMuseWeb.Models
{
    public enum ResourceType
    {
        Image,
        Mesh,
        RoomStyle
    }

    public class Resource<T>
    {
        public int ID { get; set; }
        public ResourceType Type { get; set; }
        public MetaData MetaData { get; set; }
        public T[][] Data { get; set; }
    }

    public class MetaData
    {
        public int ResourceID { get; set; }
        public string NameOfPiece { get; set; }
        public string DateOfCreation { get; set; }
        public List<Creator> Creators { get; set; }
        public string FurtherInformation { get; set; }
        public List<Source> Sources { get; set; }
        public string License { get; set; }
    }

    public class Source
    {
        public int ID { get; set; }//currently unuesd
        public string Name { get; set; }
        public string URL { get; set; }
    }

    public class Creator
    {
        public int ID { get; set; }//currently unused
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string DateOfDeath { get; set; }
    }

    public class ResourceModel
    {
        public int ID { get; set; }
        public ResourceType Type { get; set; }
        public string MetaDataJSON { get; set; }
        public byte[] Data { get; set; }
    }

    //public class MetaDataModel
    //{
    //    public int ResourceID { get; set; }
    //    public string NameOfPiece { get; set; }
    //    public string DateOfCreation { get; set; }
    //    public List<Creator> Creators { get; set; }
    //    public string FurtherInformation { get; set; }
    //    public List<Source> Sources { get; set; }
    //    public string License { get; set; }
    //    public ResourceModel Resource { get; set; }
    //}

}
