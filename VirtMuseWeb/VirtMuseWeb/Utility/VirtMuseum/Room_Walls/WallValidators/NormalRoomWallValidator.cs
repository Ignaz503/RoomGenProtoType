using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// wall validator for 1x1 rooms
/// </summary>
public class NormalRoomWallValidator : IWallValidator
{
    public bool WallNeedsRemoval(uint cornerOneCount, uint cornerTwoCount)
    {
        return false;
    }
}
