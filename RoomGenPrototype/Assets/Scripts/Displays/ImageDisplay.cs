﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageDisplay : Display {

    /// <summary>
    /// scales the x pos when setuping
    /// </summary>
    public static float XPosScale = .55f;
    
    /// <summary>
    /// the rotation of the image display
    /// </summary>
    public static Vector3 Rotation = new Vector3(0, -90f, 0);
    /// <summary>
    /// scale of the image
    /// </summary>
    public static float Scale = 0.2f;

    /// <summary>
    /// local y position of the image
    /// </summary>
    public static float YPos = -.3f;

    /// <summary>
    /// the mseh render that displays the image
    /// </summary>
    [SerializeField] RawImage[] imageDisplays;

    private void Awake()
    {
        Type = DisplayType.ImageDisplay;
    }

    public override void ApplyResource(BaseDisplayResource resource)
    {
        base.ApplyResource(resource);
        foreach(RawImage image in imageDisplays)
        {
            resource.ApplyToGameobject(image.gameObject);
        }
        //TODO
        //scale x and y a bit depending on size of texture
    }

    public override void SetUp(MuseumDisplayInfo dispInfo, GameObject parent)
    {
        transform.SetParent(parent.transform);
        float xLocPos = XPosScale * dispInfo.PositionModifier.x;

        transform.localPosition = new Vector3(xLocPos, YPos, dispInfo.PositionModifier.y);

        transform.localEulerAngles = new Vector3(Rotation.x,Rotation.y * dispInfo.PositionModifier.x,Rotation.z);
        transform.localScale = new Vector3(Scale * .75f, Scale, 0.01f);
    }

    protected override System.Type SetToDefaultInteractionBehaviour()
    {
        return null;
    }

    protected override void InteractionStarted()
    {
        //throw new System.NotImplementedException();
    }

    protected override void InteractionEnded()
    {
        //throw new System.NotImplementedException();
    }

    public override void ApplyPreProcessingInformation(PreProcessingGameObjectInformation info)
    {
        foreach(RawImage img in imageDisplays)
            info.ApplyScale(img.transform);
    }
}
