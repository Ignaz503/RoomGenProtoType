using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable {

    GameObject Object { get; }

    /// <summary>
    /// function called when an interaction is started by a player
    /// </summary>
    /// <param name="player"></param>
    void Interact(Player player);
}
