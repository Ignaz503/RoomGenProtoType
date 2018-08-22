using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTextureRequest : BaseRoomStyleRequest
{
    public FloorTextureRequest(int resLocator, GameObject reqestor)
        :base(resLocator,reqestor,RoomStyleResource.TextureToApply.Floor)
    {}
}
