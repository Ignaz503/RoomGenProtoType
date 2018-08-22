using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingTextureRequest : BaseRoomStyleRequest
{
    public CeilingTextureRequest(int resLoc, GameObject requestor)
        : base(resLoc, requestor, RoomStyleResource.TextureToApply.Ceiling)
    {}    
}
