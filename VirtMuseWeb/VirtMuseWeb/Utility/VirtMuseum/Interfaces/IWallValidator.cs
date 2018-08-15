using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtMuseWeb.Utility
{
    public interface IWallValidator
    {
        /// <summary>
        /// Checks if this wall needs to be removed for this roomtype
        /// by checking how often this corner appearse in all walls
        /// for every tile of the room
        /// </summary>
        /// <returns>true if wall is to be kept, false otherwise</returns>
        bool WallNeedsRemoval(uint cornerOneCount, uint cornerTwoCount);
    }

}