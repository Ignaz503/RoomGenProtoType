﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class MeshDisplay : Display {

    public static float XPosScale;
    public static float XPosModifier;

    public MeshFilter ParentMesh;
    public MeshFilter ChildMesh;

    private void Awake()
    {
        Type = DisplayType.MeshDisplay;

        GameObject obj = new GameObject();
        ChildMesh = obj.AddComponent<MeshFilter>();

        obj.transform.SetParent(ParentMesh.transform);
        obj.AddComponent<MeshRenderer>();
        obj.transform.localPosition = Vector3.zero;

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

    void SetUpMeshRendererOptions()
    {

        MeshRenderer re = ChildMesh.gameObject.GetComponent<MeshRenderer>();

        re.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

    }

    public override void ApplyResource(Resource resource)
    {
        base.ApplyResource(resource);
        resource.ApplyToGameobject(ChildMesh.gameObject);
        ScaleChildToFitParent();
        SetUpMeshRendererOptions();
    }

    public override void SetUp(MuseumDisplayInfo dispInfo, GameObject parent)
    {
       transform.parent.SetParent(parent.transform);


        float xLocalPos = (XPosScale * parent.transform.localScale.x) + XPosModifier;

        float yLocalPos = -.5f;

        transform.parent.localPosition = new Vector3(xLocalPos * dispInfo.PositionModifier.x, yLocalPos, dispInfo.PositionModifier.y);
    }

    protected  override void InteractionStarted()
    {
        gameObject.GetComponent<AutoMoveAndRotate>().enabled = false;
    }

    protected override void InteractionEnded()
    {
        gameObject.GetComponent<AutoMoveAndRotate>().enabled = true;
    }

    protected override Type SetToDefaultInteractionBehaviour()
    {
        return typeof(ObejctInHandInteraction);
    }
}
