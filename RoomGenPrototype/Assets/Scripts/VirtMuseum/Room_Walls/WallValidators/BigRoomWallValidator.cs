using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigRoomWallValidator : IWallValidator
{
    public bool WallNeedsRemoval(uint cornerOneCount, uint cornerTwoCount)
    {
        return cornerOneCount > 2;
    }
}
