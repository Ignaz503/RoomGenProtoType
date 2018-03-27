using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRoomPlacableChecker
{
    bool CheckIfPlacable(Vector2Int origin, out List<Vector2Int[]> possibleSequences, Museum virtMuse);
}
