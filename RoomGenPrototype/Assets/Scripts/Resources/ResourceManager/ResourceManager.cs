using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;


public sealed class ResourceManager
{
    private static ResourceManager _Instance;

    string resourcePath;
    HashSet<string> MuseumTypes;

    string ResourceEnding = ".res";
    string MetaDataEnding = ".meta";

    private ResourceManager()
    {
        //TODO: Load info not new list
        MuseumTypes = new HashSet<string>();

    }

    public static ResourceManager Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = new ResourceManager();
            return _Instance;
        }
    }

    public void AddNewMuseumType(string newType)
    {
        if (MuseumTypes.Contains(newType))
            throw new Exception($"Museum Type: {newType} already exits");

        Directory.CreateDirectory(resourcePath + newType);

        MuseumTypes.Add(newType);  
    }

    public List<string> GetPossibleMuseumTypes()
    {
        return MuseumTypes.ToList();
    }

    /// <summary>
    /// Saves a unity engine object to the corretct location
    /// </summary>
    public void AddNewResourceForType(string type,UnityEngine.Object obj, Func<UnityEngine.Object,byte[]>Serialize)
    {
        //TODO metadata serialization
        if (Serialize == null)
            throw new SerializationException("Serialization Function cann't be null");
        if (obj == null)
            throw new SerializationException("Object that needs saving can't be null");
        if(!MuseumTypes.Contains(type))
        {
            //TODO Decide just create it or throw exception
            throw new Exception($"Museum Type: {type} does not exist");
        }
        //TODO create resource locator
        string resoureceLocator = "";
        string path = resourcePath + type + @"\" + resoureceLocator + ResourceEnding;
        if (File.Exists(path))
        {
            throw new Exception("Resource already exists");
        }

        using (FileStream f = File.OpenWrite(path))
        {
            using (BinaryWriter w = new BinaryWriter(f))
            {
                w.Write(Serialize(obj));
            }
        }
        //TODO Metadata -> xml

    }

    public int GetNumberOfResourcesForType(string type)
    {
        if (!MuseumTypes.Contains(type))
            throw new Exception($"Museum Type: {type} does not exist");

        string[] files = Directory.GetFiles(resourcePath + type);
        return files.Length;
    }
    
    //TODO Get all resources
    //TODO Get random resource
    //TODO Get metadata for resource
}
