using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterImageDisplay : Display
{
    [SerializeField] Material MaterialPrefab;
    MeshRenderer[] displayMeshRenderes;

    //TODO: Maybe scale a bit depending on texture that is applied

    private void Awake()
    {
        Type = DisplayType.ImageDisplay;

        displayMeshRenderes = new MeshRenderer[transform.childCount];
        int debug = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            MeshRenderer re = child.GetComponent<MeshRenderer>();

            if(re != null)
            {
                displayMeshRenderes[i] = re;
                debug++;
            }
        }
    }

    public override void ApplyResource(Object obj)
    {
        if(obj is Texture)
        {
            Texture textToApply = obj as Texture;
            Material mat = new Material(MaterialPrefab)
            {
                mainTexture = textToApply
            };

            foreach (MeshRenderer re in displayMeshRenderes)
            {
                re.material = mat;
            }
        }
        else
            throw new System.Exception($"Trying to apply wrong resource to {GetType()}. Resourece need is Texture, trying to apply {obj.GetType()}");
    }

    public override void SetUp(MuseumDisplayInfo dispInfo, GameObject parent)
    {
        transform.parent.SetParent(parent.transform);
        transform.parent.localPosition = Vector3.zero;
    }
}
