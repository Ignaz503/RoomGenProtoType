using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RoomTypePlacableCheker
{
    List<Vector2Int[]> stepSequences;

    public RoomTypePlacableCheker(RoomType type)
    {
        stepSequences = new List<Vector2Int[]>();

        switch (type)
        {
            case RoomType.Normal:
                stepSequences.Add(new Vector2Int[] { Vector2Int.zero });
                break;
            case RoomType.Long:
                stepSequences.Add(new Vector2Int[] { Vector2Int.up });
                stepSequences.Add(new Vector2Int[] { Vector2Int.left });
                stepSequences.Add(new Vector2Int[] { Vector2Int.right });
                stepSequences.Add(new Vector2Int[] { Vector2Int.down });
                break;
            case RoomType.Big:
                stepSequences.Add(new Vector2Int[] { Vector2Int.up, Vector2Int.up + Vector2Int.right, Vector2Int.up + Vector2Int.right + Vector2Int.down });
                stepSequences.Add(new Vector2Int[] { Vector2Int.up, Vector2Int.up + Vector2Int.left, Vector2Int.up + Vector2Int.left + Vector2Int.down });
                stepSequences.Add(new Vector2Int[] { Vector2Int.down, Vector2Int.down + Vector2Int.right, Vector2Int.down + Vector2Int.right + Vector2Int.up });
                stepSequences.Add(new Vector2Int[] { Vector2Int.down, Vector2Int.down + Vector2Int.left, Vector2Int.down + Vector2Int.left + Vector2Int.up });
                break;
            case RoomType.L:
                stepSequences.Add(new Vector2Int[] { Vector2Int.up, Vector2Int.right });
                stepSequences.Add(new Vector2Int[] { Vector2Int.up, Vector2Int.left });
                stepSequences.Add(new Vector2Int[] { Vector2Int.down, Vector2Int.right });
                stepSequences.Add(new Vector2Int[] { Vector2Int.down, Vector2Int.left });
                break;
        }
    }

    public bool CheckIfPlacable(Vector2Int origin, out List<Vector2Int[]> possibleSequences, Museum virtMuse)
    {
        possibleSequences = new List<Vector2Int[]>();
        foreach (Vector2Int[] steps in stepSequences)
        {
            int free = 0;
            foreach (Vector2Int step in steps)
            {
                Vector2Int toCheck = origin + step;
                if (toCheck.x >= 0 && toCheck.x < virtMuse.Size &&
                    toCheck.y >= 0 && toCheck.y < virtMuse.Size)
                {
                    if (virtMuse.RoomMap[toCheck.x, toCheck.y] == -1)
                    {
                        free++;
                    }
                }
            }// end for step in steps

            if (free == steps.Length)
            {
                possibleSequences.Add(steps);
            }
        }//end foreach stepsequence in steps
        if (possibleSequences.Count > 0)
            return true;
        else
            return false;
    }
}