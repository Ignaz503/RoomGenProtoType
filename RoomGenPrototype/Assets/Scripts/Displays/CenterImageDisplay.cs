using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CenterImageDisplay : Display
{

    /// <summary>
    /// the mesh renderes on all sides of the pillar
    /// </summary>
    public RawImage[] displayMeshRenderes;

    //TODO: Maybe scale a bit depending on texture that is applied

    private void Awake()
    {
        Type = DisplayType.ImageDisplay;
    }

    public override void ApplyResource(BaseDisplayResource resource)
    {
        base.ApplyResource(resource);
        foreach (RawImage re in displayMeshRenderes)
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

    public override void ApplyPreProcessingInformation(PreProcessingGameObjectInformation info)
    {
        //maybe scale? maybe scale everything even gameobject base
        foreach (RawImage re in displayMeshRenderes)
            info.ApplyScale(re.transform);
    }
}
