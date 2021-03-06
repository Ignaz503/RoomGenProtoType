﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created By: Bunny83 · Nov 20, 2014 
/// User Page: https://answers.unity.com/users/6612/bunny83.html
/// Thread algorithm found: https://answers.unity.com/questions/836915/smallest-bounding-sphere.html
/// C# Adaptatipo of Ritters Alpgortitm
/// </summary>
public class BoundingSphere
{
    /// <summary>
    /// center of the bounding sphre
    /// </summary>
    public Vector3 center;

    /// <summary>
    /// radius of bounding sphere
    /// </summary>
    public float radius;

    public BoundingSphere(Vector3 aCenter, float aRadius)
    {
        center = aCenter;
        radius = aRadius;
    }

    /// <summary>
    /// calculates a bounding sphrere for a given list of vertices
    /// </summary>
    /// <param name="aPoints">the list of vertices for whom the bounding sphere should be calculated</param>
    /// <returns>the bounding sphere</returns>
    public static BoundingSphere Calculate(IEnumerable<Vector3> aPoints)
    {
        Vector3 xMin, xMax, yMin, yMax, zMin, zMax;
        xMin = yMin = zMin = Vector3.one * float.PositiveInfinity;
        xMax = yMax = zMax = Vector3.one * float.NegativeInfinity;

        foreach (var p in aPoints)
        {
            if (p.x < xMin.x)
                xMin = p;
            if (p.x > xMax.x)
                xMax = p;
            if (p.y < yMin.y)
                yMin = p;
            if (p.y > yMax.y)
                yMax = p;
            if (p.z < zMin.z)
                zMin = p;
            if (p.z > zMax.z)
                zMax = p;
        }

        float xSpan = (xMax - xMin).sqrMagnitude;
        float ySpan = (yMax - yMin).sqrMagnitude;
        float zSpan = (zMax - zMin).sqrMagnitude;

        Vector3 diameter1 = xMin;
        Vector3 diameter2 = xMax;

        float maxSpan = xSpan;
        if (ySpan > maxSpan)
        {
            maxSpan = ySpan;
            diameter1 = yMin;
            diameter2 = yMax;
        }
        if (zSpan > maxSpan)
        {
            diameter1 = zMin;
            diameter2 = zMax;
        }

        Vector3 center = (diameter1 + diameter2) * 0.5f;
        float sqRad = (diameter2 - center).sqrMagnitude;
        float radius = Mathf.Sqrt(sqRad);

        foreach (Vector3 p in aPoints)
        {
            float d = (p - center).sqrMagnitude;
            if (d > sqRad)
            {
                float r = Mathf.Sqrt(d);
                radius = (radius + r) * 0.5f;
                sqRad = radius * radius;
                float offset = r - radius;
                center = (radius * center + offset * p) / r;
            }
        }

        return new BoundingSphere(center, radius);
    }
}