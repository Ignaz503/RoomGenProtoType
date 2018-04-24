using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using UnityEngine;

[DataContract]
public class MuseumDisplayInfo
{
    [DataMember]
    public Display.DisplayType Type;
    [DataMember]
    public string AssociatedResourceLocator = "";
    [DataMember]
    public uint AssociatedRoomID;
    //should be either -1 or 1 for wall display
    // should be 1 for center display
    [DataMember]
    public Vector2 PositionModifier;

    public MuseumDisplayInfo()
    {}
}
