using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace VirtMuseWeb.Models
{
    /// <summary>
    /// Represents a creator(authirm painter etc) of a resource
    /// </summary>
    [DataContract]
    public class Creator
    {
        [DataMember] public int ID { get; set; }//currently unused
        [DataMember] public string Name { get; set; }
        [DataMember] public string DateOfBirth { get; set; }
        [DataMember] public string DateOfDeath { get; set; }

        public static implicit operator string(Creator c)
        {
            return "Name: " + c.Name + "\n"+"Born: " + c.DateOfBirth + $"\n" + (c.DateOfDeath == "" ? "" : ("Died: " + c.DateOfDeath));
        }
    }
}