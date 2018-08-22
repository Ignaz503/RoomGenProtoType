using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace VirtMuseWeb.Models
{
    /// <summary>
    /// Represents a Source where information for this resource was gotten
    /// </summary>
    [DataContract]
    public class Source
    {
        [DataMember] public int ID { get; set; }//currently unuesd
        [DataMember] public string Name { get; set; }
        [DataMember] public string URL { get; set; }

        public static implicit operator string(Source src)
        {
            return $"Name: {src.Name}\n URL: {src.URL}"; 
        }
    }

}