using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtMuseWeb.Models
{
    public enum ResourceType
    {
        Image,
        Mesh
    }

    public class Resource
    {
        public int ID { get; set; }
        public ResourceType Type { get; set; }
        public MetaData MetaData { get; set; }
        public float[] Data { get; set; }
    }

    public class MetaData
    {
        public int ResourceID { get; set; }
        public string NameOfPiece { get; set; }
        public DateTime DateOfCreation { get; set; }
        public List<Creator> Creators { get; set; }
        public string FurtherInformation { get; set; }
        public List<Source> Sources { get; set; }
        public string License { get; set; }
    }

    public class Source
    {
        public int ID;
        public string Name { get; set; }
        public string URL { get; set; }
    }

    public class Creator
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfDeath { get; set; }
    }
}
