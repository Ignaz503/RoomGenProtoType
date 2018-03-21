using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageDisplay : Display {

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
            throw new System.Exception("Trying to apply wron resource to ImgaeDisplay");
    }
}
