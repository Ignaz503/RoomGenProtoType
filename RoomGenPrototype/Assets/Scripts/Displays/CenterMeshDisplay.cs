using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CenterMeshDisplay : MeshDisplay {

    /// <summary>
    /// position and parenting setup
    /// </summary>
    public override void SetUp(MuseumDisplayInfo dispInfo, GameObject parent)
    {
        transform.parent.SetParent(parent.transform);
        transform.parent.localPosition = Vector3.zero;
    }
}
