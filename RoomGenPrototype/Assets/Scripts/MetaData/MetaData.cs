using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using MetaDataDisplayStyles;

[DataContract]
public class MetaData
{
    [DataMember]
    public string ResourceID;

    [DataMember]
    public List<string> associatedResourceIDs;

    [DataMember]
    [MetaDataAttribute("Creator")]
    public List<string> Creators;

    [DataMember]
    [MetaDataAttribute("Date")]
    public string DateOfCreation;

    [DataMember]
    [MetaDataAttribute("FurtherInformation")]
    public Dictionary<int, string> FurterInformationAccordingToLOD;

    [DataMember]
    [MetaDataAttribute("Sources")]
    public List<string> Sources;
     
    [DataMember]
    [MetaDataAttribute("License")]
    public string License;

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

}
