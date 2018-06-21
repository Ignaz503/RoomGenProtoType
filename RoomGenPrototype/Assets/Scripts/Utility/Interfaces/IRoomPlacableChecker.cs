using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRoomPlacableChecker
{
    /// <summary>
    /// function that checks if a room is placable at this location in the museum
    /// when the musem is gnerated
    /// </summary>
    /// <param name="origin">the origin position of the room</param>
    /// <param name="possibleSequences">out of all the poss directions of steps to build this room type</param>
    /// <param name="virtMuse">the virtual museum the room should be placed in</param>
    /// <returns>ture if room is placable, false if not</returns>
    bool CheckIfPlacable(Vector2Int origin, out List<Vector2Int[]> possibleSequences, Museum virtMuse);
}
