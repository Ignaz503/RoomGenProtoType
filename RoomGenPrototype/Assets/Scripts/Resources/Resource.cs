using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// base class for resource
/// </summary>
public abstract class Resource
{
    public string ResourecID { get; protected set; }
    public MetaData MetaData { get; protected set; }
    public string ResrourceType { get; protected set; }
    //TODO Interaction behaviour
    public string InteractionBehaviour = "";

    public abstract void ApplyToGameobject(GameObject obj);
}

/// <summary>
/// a resource that is a mesh
/// </summary>
public class MeshResource : Resource
{
    Mesh mesh;
    Material mat;

    public MeshResource(Mesh msh, Material mat)
    {
        mesh = msh;
        this.mat = mat;
    }

    public override void ApplyToGameobject(GameObject obj)
    {
        MeshFilter filter = obj.GetComponent<MeshFilter>();
        if(filter == null)
        {
            filter = obj.AddComponent<MeshFilter>();
        }
        filter.mesh = mesh;

        MeshRenderer re = obj.GetComponent<MeshRenderer>();
        if(re == null)
            re = obj.AddComponent<MeshRenderer>();

        re.material = mat;
    }
}

/// <summary>
/// resource that is a image
/// </summary>
public class ImageResource : Resource
{
    Texture2D image;

    public ImageResource(Texture2D img)
    {
        image = img;
    }

    public override void ApplyToGameobject(GameObject obj)
    {
        MeshRenderer re = obj.GetComponent<MeshRenderer>();
        if(re == null)
        {
            re = obj.AddComponent<MeshRenderer>();
        }

        Material mat = new Material(Shader.Find("Sprites/Default"))
        {
            mainTexture = image as Texture
        };

        re.material = mat;
    }


}
