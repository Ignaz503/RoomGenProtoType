using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInteractionContainer
{
    protected Type interactionBehaviour;

    public BaseInteractionContainer(Type t)
    {
        //WARMING CHANGES TO THIS CTOR MEAN CHANGES TO THE FACTORY
        interactionBehaviour = t;
    }

    private BaseInteractionContainer(){}

    public abstract void ApplyAndStartInteraction(GameObject interActivator, GameObject interActedUpon);

    public abstract void EndInteraction();

    public override string ToString()
    {
        return $"{GetType()} contains behaviour {interactionBehaviour} ";
    }

}

public abstract class SingleObjectInteractionContainer : BaseInteractionContainer
{
    protected GameObject objContainingBehaviour;

    public SingleObjectInteractionContainer(Type t): base(t)
    {}

    public override void EndInteraction()
    {
        GameObject.Destroy(objContainingBehaviour.GetComponent(interactionBehaviour));
    }
}

public abstract class DoubleObjectInteractionContainer : BaseInteractionContainer
{
    GameObject[] objectsContainingBehaviour;

    public DoubleObjectInteractionContainer(Type t) : base(t)
    {
        objectsContainingBehaviour = new GameObject[2];
    }

    public override void EndInteraction()
    {
        foreach(GameObject obj in objectsContainingBehaviour)
        {
            GameObject.Destroy(obj.GetComponent(interactionBehaviour));
        }
    }
    protected void SetObjectContainingBehaviour(GameObject one, GameObject two)
    {
        objectsContainingBehaviour[0] = one;
        objectsContainingBehaviour[0] = two;
    }
}

public class ActivatorSideInteractionContainer : SingleObjectInteractionContainer
{

    public ActivatorSideInteractionContainer(Type t) :base(t)
    {}

    public override void ApplyAndStartInteraction(GameObject interActivator, GameObject interActedUpon)
    {
        objContainingBehaviour = interActivator;

        Interaction interaction = interActivator.AddComponent(interactionBehaviour) as Interaction;

        interaction.StartInteraction(interActivator, interActedUpon);
    }

}

public class ActedUponSideInteractionContainer : SingleObjectInteractionContainer
{
    public ActedUponSideInteractionContainer(Type t) : base(t)
    {}

    public override void ApplyAndStartInteraction(GameObject interActivator, GameObject interActedUpon)
    {
        objContainingBehaviour = interActedUpon;
        dynamic interaction = interActedUpon.AddComponent(interactionBehaviour);
        interaction.StartInteraction(interActivator, interActedUpon);
    }
}

public class BothSidesInteractionContainer : DoubleObjectInteractionContainer
{
    public BothSidesInteractionContainer(Type t) : base(t)
    {}

    public override void ApplyAndStartInteraction(GameObject interActivator, GameObject interActedUpon)
    {
        SetObjectContainingBehaviour(interActivator, interActedUpon);

        dynamic interaction = interActivator.AddComponent(interactionBehaviour);

        interaction.StartInteraction(interActivator, interActedUpon);

        dynamic uponInteraction = interActedUpon.AddComponent(interactionBehaviour);

        uponInteraction.StartInteraction(interActivator, interActedUpon);
    }
}

public class NewInteractionContainer : SingleObjectInteractionContainer
{
    public NewInteractionContainer(Type t) : base(t)
    {}

    public override void ApplyAndStartInteraction(GameObject interActivator, GameObject interActedUpon)
    {
       objContainingBehaviour = new GameObject();

        dynamic interaction = objContainingBehaviour.AddComponent(interactionBehaviour);
        interaction.StartInteraction(interActivator, interActedUpon);
    }
}