using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Class for any interaction that should be possible between two gameobjects
/// the child class needs to define a InteractionAttribute 
/// with one of the InteractionContainer types as its type or it won't be found
/// </summary>
public abstract class Interaction : MonoBehaviour
{
    public abstract void StartInteraction(GameObject activator, GameObject interactedUpon);
}

[AttributeUsage(AttributeTargets.Class)]
public class InteractionAttribute : Attribute
{
    public Type ContainerType { get; protected set; }

    public InteractionAttribute(Type containerType)
    {
        ContainerType = containerType;
    }

    public override int GetHashCode()
    {
        return ContainerType.GetHashCode();
    }

    public override string ToString()
    {
        return ContainerType.ToString();
    }

}

