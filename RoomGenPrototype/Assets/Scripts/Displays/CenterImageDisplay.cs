using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterImageDisplay : Display
{
    /// <summary>
    /// Material prefab used to apply texture
    /// </summary>
    [SerializeField] Material MaterialPrefab;

    /// <summary>
    /// the mesh renderes on all sides of the pillar
    /// </summary>
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

    public override void ApplyResource(BaseDisplayResource resource)
    {
        base.ApplyResource(resource);
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

    protected override System.Type SetToDefaultInteractionBehaviour()
    {
        return null;
    }

    protected override void InteractionStarted()
    {
        throw new System.NotImplementedException();
    }

    protected override void InteractionEnded()
    {
        //throw new System.NotImplementedException();
    }
}
