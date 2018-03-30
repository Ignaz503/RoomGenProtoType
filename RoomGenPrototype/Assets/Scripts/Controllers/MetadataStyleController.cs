using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;
using MetaDataDisplayStyles;

public class MetadataStyleController : MonoBehaviour {
    [Serializable]
    public class MetadataAttrToStyleMapping
    {
        [SerializeField] public string AttributeName;
        [SerializeField] public List<string> Styles;
        [SerializeField]public Style.StyleApplicationType ApllicationStyle;
    }

    public static MetadataStyleController Instance;
    [SerializeField] public List<MetadataAttrToStyleMapping> MetaDataTypeToStyleMapping;
    Dictionary<MetaDataAttribute, Style> Mapping;
    private void Awake()
    {
        if (Instance != null)
        {
            throw new Exception("There already exists a metadata style controller");
        }
        Instance = this;

        Mapping = new Dictionary<MetaDataAttribute, Style>();

        //create mapping for eaasy access
        foreach(MetadataAttrToStyleMapping ma in MetaDataTypeToStyleMapping)
        {
            Mapping.Add(new MetaDataAttribute(ma.AttributeName), new Style(ma.ApllicationStyle,ma.Styles));
        }

    }

}
