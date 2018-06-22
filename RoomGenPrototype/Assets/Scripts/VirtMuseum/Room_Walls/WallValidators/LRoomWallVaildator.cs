using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// wall validator for L-shaped rooms(2x2 -1 rooms)
/// </summary>
public class LRoomWallVaildator : IWallValidator
{
    public bool WallNeedsRemoval(uint cornerOneCount, uint cornerTwoCount)
    {
        return cornerOneCount > 2 && cornerTwoCount > 1;
    }
}
