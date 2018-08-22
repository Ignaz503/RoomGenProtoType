using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleWallTextureRequest :BaseRoomStyleRequest
{
    public SingleWallTextureRequest(int resLoc, GameObject requestor)
        : base(resLoc, requestor, RoomStyleResource.TextureToApply.Wall)
    {}    
}
