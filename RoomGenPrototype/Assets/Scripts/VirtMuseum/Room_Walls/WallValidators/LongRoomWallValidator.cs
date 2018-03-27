using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRoomWallValidator : IWallValidator
{
    public bool WallNeedsRemoval(uint cornerOneCount, uint cornerTwoCount)
    {
        return cornerOneCount > 1 && cornerTwoCount > 1;
    }
}
