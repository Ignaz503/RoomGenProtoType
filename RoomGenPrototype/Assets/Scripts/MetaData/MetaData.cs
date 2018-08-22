using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using TMPro;

/// <summary>
/// metadata related to resource containt further information
/// </summary>
[DataContract]
public class MetaData
{
    public MetaData()
    {
        FurterInformationAccordingToLOD = new Dictionary<int, string>();
        Sources = new List<string>();
        Creators = new List<string>();
    }

    [DataMember]
    public string ResourceID { get; set; }

    [DataMember]
    public List<string> associatedResourceIDs { get; set; }

    [DataMember]
    [MetaDataAttribute("Name", MetaDataAttribute.MetaDataDisplayType.FirstPage)]
    public string NameOfPiece { get; set; }

    [DataMember]
    [MetaDataAttribute("Creator",MetaDataAttribute.MetaDataDisplayType.FirstPage)]
    public List<string> Creators { get; set; }

    [DataMember]
    [MetaDataAttribute("Date", MetaDataAttribute.MetaDataDisplayType.FirstPage)]
    public string DateOfCreation { get; set; }


    [DataMember]
    [MetaDataAttribute("FurtherInfo", MetaDataAttribute.MetaDataDisplayType.NewPage)]
    public Dictionary<int, string> FurterInformationAccordingToLOD { get; set; }

    [DataMember]
    [MetaDataAttribute("Sources", MetaDataAttribute.MetaDataDisplayType.NewPage)]
    public List<string> Sources { get; set; }

    [DataMember]
    [MetaDataAttribute("License", MetaDataAttribute.MetaDataDisplayType.NewPage)]
    public string License { get; set; }

    #region Gamification
    //[DataMember]
    //public HashSet<string> QuizzableWords;

    //[DataMember]
    //public List<Tuple<string, string>> Questions;
    #endregion

    public string Serialize()
    {
        MemoryStream stream = new MemoryStream();
        DataContractSerializer serializer = new DataContractSerializer(typeof(MetaData));
        XmlWriterSettings settings = new XmlWriterSettings() { Indent = true };

        using (XmlWriter writer = XmlWriter.Create(stream, settings))
            serializer.WriteObject(writer, this);

        stream.Position = 0;
        StreamReader reader = new StreamReader(stream);
        string data = reader.ReadToEnd();
        reader.Close();
        stream.Close();
        return data;
    }

    public static MetaData Deserialize(string data)
    {
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        DataContractSerializer serializer = new DataContractSerializer(typeof(MetaData));
        MetaData request = serializer.ReadObject(stream) as MetaData;
        stream.Close();
        return request;
    }

    public string GetFieldWithAttributeTypeAsString(string AttributeType, int proficency = -1)
    {
        if(proficency != -1)
        {
            if (GetType().GetFields().Where(
                (inf) =>
                {
                    return (inf.GetCustomAttribute<MetaDataAttribute>() as MetaDataAttribute).Type == AttributeType;
                }) != null)
            {
                if (FurterInformationAccordingToLOD.ContainsKey(proficency))
                {
                    return FurterInformationAccordingToLOD[proficency];
                }// end if contains key
                else
                {
                    return FurterInformationAccordingToLOD.Values.First();
                }// end else
            }// end attribute type exists
            else
                return "";
        }// end if profincy not equal -1
        else
        {
            return (GetType().GetFields().Where(
                (inf) => { return (inf.GetCustomAttribute<MetaDataAttribute>() as MetaDataAttribute).Type == AttributeType; }
                ) as FieldInfo).GetValue(null).ToString();
        }
    }

}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class MetaDataAttribute : Attribute
{
    /// <summary>
    /// Defines if a metadata member should be created as a text field on the
    /// first page of the metadata display
    /// or as a button that generates a new page that is filled
    /// </summary>
    public enum MetaDataDisplayType
    {
        FirstPage,
        NewPage
    }

    public string Type { get; private set; }
    public MetaDataDisplayType DisplayType { get; private set; }
    

    public MetaDataAttribute(string type,MetaDataDisplayType dispT)
    {
        Type = type;
        DisplayType = dispT;
    }

    public override int GetHashCode()
    {
        return Type.GetHashCode();
    }

}