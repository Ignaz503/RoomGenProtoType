using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageDisplay : Display {

    public static float XPosScale = .55f;
    public static Vector3 Rotation = new Vector3(0, 90f, 0);
    public static float Scale = 0.2f;
    public static float YPos = -.3f;

    public SpriteRenderer[] PictureDisplays;

    private void Awake()
    {
        Type = DisplayType.ImageDisplay;

        PictureDisplays = GetComponentsInChildren<SpriteRenderer>();

    }

    public override void ApplyResource(UnityEngine.Object obj)
    {
        if (obj is Sprite)
        {
            Sprite sprite = obj as Sprite;
            foreach(SpriteRenderer spRen in PictureDisplays)
            {
                spRen.sprite = sprite;
            }
        }
        else
            throw new System.Exception("Trying to apply wrong resource to ImgaeDisplay");
    }

    public override void SetUp(MuseumDisplayInfo dispInfo, GameObject parent)
    {
        transform.SetParent(parent.transform);
        float xLocPos = XPosScale * dispInfo.PositionModifier.x;

        transform.localPosition = new Vector3(xLocPos, YPos, dispInfo.PositionModifier.y);

        transform.localEulerAngles = Rotation;
        transform.localScale = new Vector3(Scale * .75f, Scale, 1);
    }
}
