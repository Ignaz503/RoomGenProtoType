using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCPlayerHand : PlayerHand
{
    [SerializeField] Vector3 RestPosition;
    [SerializeField] Vector3 InfrontOfFacePosition;
    [SerializeField] KeyCode MoveHandKey;

    public override void DropObject()
    {
        throw new System.NotImplementedException();
    }

    public override void GrabObject(IHoldableObject holdObj)
    {
        
    }
}
