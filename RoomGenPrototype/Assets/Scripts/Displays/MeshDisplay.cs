using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDisplay : Display {

    public static float XPosScale;
    public static float XPosModifier;

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

        Vector3 locScale = gameObject.transform.localScale;
        float avg = (locScale.x + locScale.y + locScale.z) / 3f;
        float scale = (boundsParent.radius / boundsChild.radius)*avg;
        


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

    public override void SetUp(MuseumDisplayInfo dispInfo, GameObject parent)
    {
       transform.parent.SetParent(parent.transform);


        float xLocalPos = (XPosScale * parent.transform.localScale.x) + XPosModifier;

        float yLocalPos = -.5f;

        transform.parent.localPosition = new Vector3(xLocalPos * dispInfo.PositionModifier.x, yLocalPos, dispInfo.PositionModifier.y);
    }
}
