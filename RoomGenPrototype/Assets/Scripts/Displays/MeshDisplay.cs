using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDisplay : Display {

    public MeshFilter ParentMesh;
    public MeshFilter ChildMesh;

    private void Awake()
    {
        Type = DisplayType.MeshDisplay;
    }

    public void ScaleChildToFitParent()
    {

        BoundingSphere boundsChild = BoundingSphere.Calculate(ChildMesh.sharedMesh.vertices);
        BoundingSphere boundsParent = BoundingSphere.Calculate(ParentMesh.sharedMesh.vertices);

        //scale = boundsSphere.extents.magnitude / boundsMesh.extents.magnitude;

        float scale = (boundsParent.radius / boundsChild.radius)*1.5f;//????

        //Debug.Log(boundsChild.radius);
        //Debug.Log(boundsParent.radius);
        //Debug.Log(scale);

        ChildMesh.gameObject.transform.localScale *= scale;
    }

    public override void ApplyResource(UnityEngine.Object obj)
    {
        if ((obj is MeshFilter))
        {
            ChildMesh = obj as MeshFilter;
            ChildMesh.transform.SetParent(ParentMesh.transform);
            ChildMesh.transform.localPosition =  Vector3.zero;
            ScaleChildToFitParent();
        }
        else
            throw new Exception("Trying to apply non valid resource to MeshDisplay");
    }
}
