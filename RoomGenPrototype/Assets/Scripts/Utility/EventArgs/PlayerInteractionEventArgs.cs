using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// base event arguments when a player interacts with somthing interactable
/// </summary>
public abstract class PlayerInteractionEventArgs
{
    public enum InteractingWith
    {
        Display
    }

    public InteractingWith InteractionType { get; protected set; }
    public Player InteractingPlayer { get; protected set; }
    public GameObject ObjectInteractedWith { get { return InteractedObject.Object; } }
    public IInteractable InteractedObject { get; protected set; }

    public PlayerInteractionEventArgs(Player player, IInteractable obj, InteractingWith type)
    {
        InteractingPlayer = player;
        InteractedObject = obj;
        InteractionType = type;
    }

    public Vector3 PlayerPosition { get { return InteractingPlayer.transform.position; } }

    public Vector3 ObjectPosition { get { return ObjectInteractedWith.transform.position; } }

    public Transform PlayerTransform { get { return InteractingPlayer.transform; } }

    public Transform ObjectTransform { get { return ObjectInteractedWith.transform; } }

    public Camera PlayerCamera { get { return InteractingPlayer.PlayerCam; } }

}

/// <summary>
/// event arguments when player is interacting with a dispaly
/// </summary>
public class PlayerDisplayInteractionEventArgs : PlayerInteractionEventArgs
{
    public Display DisplayInteractedWith { get; protected set; }
    
    public PlayerDisplayInteractionEventArgs(Player player, Display disp) : base(player,disp as IInteractable, InteractingWith.Display)
    {
        DisplayInteractedWith = disp;
    }

    public Display.DisplayType InteractedDisplayType { get { return DisplayInteractedWith.Type; } }


}