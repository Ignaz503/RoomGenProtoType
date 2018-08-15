using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtMuseWeb.Utility
{
    /// <summary>
    /// wall validator for 2x2 rooms
    /// </summary>
    public class BigRoomWallValidator : IWallValidator
    {
        public bool WallNeedsRemoval(uint cornerOneCount, uint cornerTwoCount)
        {
            return cornerOneCount > 2;
        }
    }
}