using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateUpdateRotationFollower : MonoBehaviour {

    [SerializeField] Transform toTrack;

    private void LateUpdate()
    {
        transform.rotation = toTrack.rotation;
    }
}
