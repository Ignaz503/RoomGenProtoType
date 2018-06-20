using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable {

    GameObject Object { get; }

    void Interact(Player player);
}
