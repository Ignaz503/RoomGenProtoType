using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRoomWallVaildator : IWallValidator
{
    public bool WallNeedsRemoval(uint cornerOneCount, uint cornerTwoCount)
    {
        return cornerOneCount > 2 && cornerTwoCount > 1;
    }
}
