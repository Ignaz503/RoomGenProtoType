using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// base class for any resource be it display resource or other
/// </summary>
public abstract class BaseResource
{
    /// <summary>
    /// The resource ID
    /// </summary>
    public string ResourecID { get; protected set; }

    /// <summary>
    /// The type of resource
    /// currently:
    ///     MeshDisplay
    ///     ImageDisplay
    ///     Texture
    /// </summary>
    public string ResrourceType { get; protected set; }

    /// <summary>
    /// function that applies a resource to a gamobject
    /// </summary>
    /// <param name="obj">the object the resource is applied to</param>
    public abstract void ApplyToGameobject(GameObject obj);
}


/// <summary>
/// base class for resource
/// </summary>
public abstract class BaseDisplayResource : BaseResource
{
    /// <summary>
    /// The Metadata associated with this display resource
    /// </summary>
    public MetaData MetaData { get; protected set; }

    /// <summary>
    /// The interaction behaviour the display wants to provide
    /// </summary>
    public string InteractionBehaviour = "";
}

/// <summary>
/// a resource that is a mesh
/// </summary>
public class DisplayMeshResource : BaseDisplayResource
{
    public Mesh Mesh { get; set; }
    public Material Mat { get; set; }

    public DisplayMeshResource()
    {

    }

    public DisplayMeshResource(Mesh msh, Material mat)
    {
        Mesh = msh;
        this.Mat = mat;
    }

    public override void ApplyToGameobject(GameObject obj)
    {
        MeshFilter filter = obj.GetComponent<MeshFilter>();
        if(filter == null)
        {
            filter = obj.AddComponent<MeshFilter>();
        }
        filter.mesh = Mesh;

        MeshRenderer re = obj.GetComponent<MeshRenderer>();
        if(re == null)
            re = obj.AddComponent<MeshRenderer>();

        re.material = Mat;
    }
}

/// <summary>
/// resource that is a image that needs to be displayed
/// </summary>
public class DisplayImageResource : BaseDisplayResource
{
    public Texture2D Image { get; set; }

    /// <summary>
    /// no param ctor, does not initialize anything
    /// </summary>
    public DisplayImageResource()
    {

    }

    /// <summary>
    /// ctor that sets the texture of the resource
    /// </summary>
    /// <param name="img">the texture of the resource</param>
    public DisplayImageResource(Texture2D img)
    {
        Image = img;
    }

    /// <summary>
    /// applies texture to gamobject by getting the meshrenderer of the object
    /// or adding one if it does not exist
    /// and setting the material of the render to a new material
    /// with the defualt sprites shader and the Image as texture
    /// </summary>
    /// <param name="obj"></param>
    public override void ApplyToGameobject(GameObject obj)
    {
        MeshRenderer re = obj.GetComponent<MeshRenderer>();
        if(re == null)
        {
            re = obj.AddComponent<MeshRenderer>();
        }

        Material mat = new Material(Shader.Find("Sprites/Default"))
        {
            mainTexture = Image 
        };

        re.material = mat;
    }

}

/// <summary>
/// resource that is a texture
/// </summary>
public class TextureResource : BaseResource
{
    /// <summary>
    /// The texture that should be applied to a mesh
    /// </summary>
    public Texture2D Image { get; set; }

    /// <summary>
    /// No param ctor that doens't initialize anything
    /// </summary>
    public TextureResource()
    {}

    /// <summary>
    /// Ctor that sets the Image of this resource
    /// </summary>
    /// <param name="image"></param>
    public TextureResource(Texture2D image)
    {
        Image = image;
    }

    /// <summary>
    /// gets mesh renderer of object, or adds one
    /// sets the material texture to Image
    /// if created Material created will have Standard shader
    /// </summary>
    /// <param name="obj"></param>
    public override void ApplyToGameobject(GameObject obj)
    {
        MeshRenderer re = obj.GetComponent<MeshRenderer>();
        if(re == null)
        {
            re = obj.AddComponent<MeshRenderer>();
        }

        re.material.mainTexture = Image;
    }
}
