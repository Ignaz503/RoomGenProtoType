using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class MeshDisplay : Display {

    public static float XPosScale;
    public static float XPosModifier;

    public Type InteractionBehaviour = typeof(ObjectInHandInteraction);

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

    void SetUpMeshRendererOptions()
    {

        MeshRenderer re = ChildMesh.gameObject.GetComponent<MeshRenderer>();

        re.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

    }

    public override void ApplyResource(UnityEngine.Object obj)
    {
        if ((obj is MeshFilter))
        {
            ChildMesh = obj as MeshFilter;
            ChildMesh.transform.SetParent(ParentMesh.transform);
            ChildMesh.transform.localPosition =  Vector3.zero;
            ScaleChildToFitParent();
            SetUpMeshRendererOptions();
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

    public override void Interact(Player player)
    {
        base.Interact(player);

        dynamic interaction = player.gameObject.AddComponent(InteractionBehaviour);

        interaction.StartInteraction(player, ChildMesh.gameObject.transform);

        gameObject.GetComponent<AutoMoveAndRotate>().enabled = false;
    }

    public override void OnInteractionEnded(PlayerInteractionEventArgs arg)
    {
        if(arg.InteractionType == PlayerInteractionEventArgs.InteractingWith.Display)
        {
            Debug.Log("this be working");
            PlayerDisplayInteractionEventArgs dispArgs = arg as PlayerDisplayInteractionEventArgs;

            //check if we are this
            if(dispArgs.DisplayInteractedWith == this)
            {
                Destroy(arg.InteractingPlayer.gameObject.GetComponent<ObjectInHandInteraction>());
            }

            ChildMesh.transform.localEulerAngles = Vector3.zero;
            gameObject.GetComponent<AutoMoveAndRotate>().enabled = true;

            //remove self from event
            arg.InteractingPlayer.OnInteractionEnd -= OnInteractionEnded;

        }
    }
}
