using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using UnityEngine;


[DataContract]
public class MuseumDisplayInfo
{
    /// <summary>
    /// type of display
    /// </summary>
    [DataMember]
    public DisplayType Type;
    /// <summary>
    /// locator to this resource so that server can send it to client
    /// </summary>
    [DataMember]
    public int AssociatedResourceLocator;
    /// <summary>
    /// Room ID where dispay is in(even when wall display)
    /// </summary>
    [DataMember]
    public uint AssociatedRoomID;
    //should be either -1 or 1 for wall display
    // should be 1 for center display
    /// <summary>
    /// position modication of display gameobject
    /// </summary>
    [DataMember]
    public Vector2 PositionModifier;

    public MuseumDisplayInfo()
    { }
}
