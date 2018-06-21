using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RotateRelative
{
    World,
    Local
}

/// <summary>
/// extension methods for Unity transform class
/// </summary>
public static class TransformExtensionMethods
{
    /// <summary>
    /// rotates a transform rleative to a camera
    /// </summary>
    /// <param name="objToRotate">object to rotate</param>
    /// <param name="camRelativeTo">camera the object need relative rotating to</param>
    /// <param name="rotateLeftRight"> left right rotation</param>
    /// <param name="rotateUpDown">up down rotation</param>
    /// <param name="relative">decide if world or local rotation modified</param>
    public static void RotateRelativeToCamera(this Transform objToRotate, Camera camRelativeTo, float rotateLeftRight, float rotateUpDown, RotateRelative relative)
    {
        Vector3 relativeUp = camRelativeTo.transform.TransformDirection(Vector3.up);
        Vector3 relativeRight = camRelativeTo.transform.TransformDirection(Vector3.right);

        Vector3 objRelativeUp = objToRotate.transform.InverseTransformDirection(relativeUp);
        Vector3 objRelativeRight = objToRotate.transform.InverseTransformDirection(relativeRight);

         Quaternion rot = Quaternion.AngleAxis(rotateLeftRight / objToRotate.localScale.x, objRelativeUp) * Quaternion.AngleAxis(-rotateUpDown / objToRotate.localScale.x, objRelativeRight);

        switch (relative)
        {
            case RotateRelative.Local:
                objToRotate.localRotation *= rot;
                break;
            case RotateRelative.World:
                objToRotate.localRotation *= rot;
                break;
            default:
                throw new System.Exception("No Relative Rotation To defined");
        }

    }

}
