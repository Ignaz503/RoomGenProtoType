using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// wall validator for 1x2 rooms
/// </summary>
public class LongRoomWallValidator : IWallValidator
{
    public bool WallNeedsRemoval(uint cornerOneCount, uint cornerTwoCount)
    {
        return cornerOneCount > 1 && cornerTwoCount > 1;
    }
}
