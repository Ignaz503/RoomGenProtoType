using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[DataContract]
public class MuseumTextureInfo
{

    /// <summary>
    /// The resource locator to get the texture from the server
    /// (aka id of texture in database)
    /// </summary>
    [DataMember]
    public string AssociatedResourceLocators = "";

    /// <summary>
    /// ID of the associated wall or room
    /// wall ids and room ids differ
    /// aka string here so that we can figure out what format it has
    /// when needed
    /// </summary>
    [DataMember]
    public string AssociatedID = "";

    /// <summary>
    /// Defines if texture starts from 0 to half widht,
    /// of from half width to width
    /// when used by walls
    /// for flloors and ceilings it has no meaning
    /// </summary>
    [DataMember]
    public int PositionModifier;

    public MuseumTextureInfo()
    {}
}
