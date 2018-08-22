using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using System;

public enum MuseumSize
{
    Small = 3,
    Medium = 5,
    Large = 10
}

[DataContract]
[Serializable]
public class MuseumRequestData
{

    /// <summary>
    /// defines what type of musem 
    /// eg. art sciences and so on
    /// </summary>
    [DataMember]
    [SerializeField]
    public string MuseumType { get; set; }

    /// <summary>
    /// defines size of museum limiting displays and rooms
    /// </summary>
    [DataMember]
    [SerializeField]
    public MuseumSize Size { get; set; }

    /// <summary>
    /// serializes it to xml
    /// </summary>
    /// <returns></returns>
    public string Serialize()
    {
        MemoryStream stream = new MemoryStream();
        DataContractSerializer serializer = new DataContractSerializer(typeof(MuseumRequestData));
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

    /// <summary>
    /// from xml to museum request
    /// </summary>
    public static MuseumRequestData Deserialize(string data)
    {
        MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        DataContractSerializer serializer = new DataContractSerializer(typeof(MuseumRequestData));
        MuseumRequestData request = serializer.ReadObject(stream) as MuseumRequestData;
        stream.Close();
        return request;
    }
}
