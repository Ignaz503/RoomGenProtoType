using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for traansform inforation thaat needs to be applied to a gamobject
/// after a resource has been preprocessed
/// currently only transform modifications possible
/// </summary>
public class PreProcessingGameObjectInformation
{ 
    /// <summary>
    /// The position value that needs to be added
    /// can be local or global -> decided in ApplyPosition
    /// /// </summary>
    public Vector3 Position { protected get; set; }

    /// <summary>
    /// The rotation that needs to be applied to the gameobject
    /// can be local or global (same as position)
    /// </summary>
    public Quaternion Rotation { protected get; set; }

    /// <summary>
    /// The scaling that needs to be applied to the gameobject
    /// caan be local or global (same as position or rotation)
    /// </summary>
    public Vector3 Scale { protected get; set; }

    /// <summary>
    /// Applies the position value to a transform
    /// (adds vector to transform position)
    /// </summary>
    /// <param name="t">the transform we want to modifiy</param>
    /// <param name="space"> if local position or global position</param>
    public void ApplyPosition(Transform t, Space space = Space.Self)
    {
        switch (space)
        {
            case Space.Self:
                t.localPosition += Position;
                break;
            case Space.World:
                t.position += Position;
                break;
        }
    }

    /// <summary>
    /// applies the rotation value to a transform
    /// (quaternion multiplicaaation)
    /// </summary>
    /// <param name="t">the transform we want to modify</param>
    /// <param name="space">if local or global rotation</param>
    public void ApplyRotation(Transform t, Space space = Space.Self)
    {
        switch (space)
        {
            case Space.Self:
                t.localRotation *= Rotation;
                break;
            case Space.World:
                t.rotation *= Rotation;
                break;
        }
    }

    /// <summary>
    /// applies scale to transform
    /// (element wise multiplication)
    /// </summary>
    /// <param name="t">the transform we want to modifiy</param>
    /// <param name="space">if local or lossy global</param>
    public void ApplyScale(Transform t, Space space = Space.Self)
    {
        switch (space)
        {
            case Space.Self:
                t.localScale.Scale(Scale);
                break;
            case Space.World:
                t.lossyScale.Scale(Scale);
                break;
        }
    }

    /// <summary>
    /// returns a PreProcessingGemobjectInformation where
    /// all the members are the neutral element of their respective
    /// application operation
    /// 0,0,0 for position
    /// identity matrix for quaaternion
    /// 1,1,1 for scale
    /// </summary>
    /// <returns></returns>
    public static PreProcessingGameObjectInformation Neutral()
    {
        return new PreProcessingGameObjectInformation()
        {
            Position = Vector3.zero,
            Rotation = Quaternion.identity,
            Scale = Vector3.one
        };
    }

}
