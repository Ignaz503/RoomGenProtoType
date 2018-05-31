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
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            MeshRenderer re = child.GetComponent<MeshRenderer>();

            if(re != null)
            {
                displayMeshRenderes[i] = re;
            }
        }
    }

    public override void ApplyResource(Resource resource)
    {
        foreach (MeshRenderer re in displayMeshRenderes)
        {
            resource.ApplyToGameobject(re.gameObject);
        }
    }

    public override void SetUp(MuseumDisplayInfo dispInfo, GameObject parent)
    {
        transform.parent.SetParent(parent.transform);
        transform.parent.localPosition = Vector3.zero;
    }

    protected override void SetToDefaultInteractionBehaviour()
    {
        InteractionBehaviour = null;
    }
}
