using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalRoomWallValidator : IWallValidator
{
    public bool WallNeedsRemoval(uint cornerOneCount, uint cornerTwoCount)
    {
        return false;
    }
}
