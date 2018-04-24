using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageDisplay : Display {

    public static float XPosScale = .55f;
    public static Vector3 Rotation = new Vector3(0, -90f, 180);
    public static float Scale = 0.2f;
    public static float YPos = -.3f;

    [SerializeField] Material materialPrefab;
    MeshRenderer meshRenderer;

    private void Awake()
    {
        Type = DisplayType.ImageDisplay;

        meshRenderer = GetComponent<MeshRenderer>();

    }

    public override void ApplyResource(UnityEngine.Object obj)
    {
        if (obj is Texture)
        {
            Material mat = new Material(materialPrefab)
            {
                mainTexture = obj as Texture
            };

            meshRenderer.material = mat;
            //TODO
            //scale x and y a bit depending on size of texture
        }
        else
            throw new System.Exception("Trying to apply wrong resource to ImgaeDisplay");
    }

    public override void SetUp(MuseumDisplayInfo dispInfo, GameObject parent)
    {
        transform.SetParent(parent.transform);
        float xLocPos = XPosScale * dispInfo.PositionModifier.x;

        transform.localPosition = new Vector3(xLocPos, YPos, dispInfo.PositionModifier.y);

        transform.localEulerAngles = new Vector3(Rotation.x,Rotation.y * dispInfo.PositionModifier.x,Rotation.z);
        transform.localScale = new Vector3(Scale * .75f, Scale, 0.01f);
    }
}
