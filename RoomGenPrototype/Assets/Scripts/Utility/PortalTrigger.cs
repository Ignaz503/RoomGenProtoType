using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class that triggers the player to be placed into the museum
/// </summary>
public class PortalTrigger : MonoBehaviour {

    /// <summary>
    /// aribiter if teleportation works or not
    /// </summary>
    [SerializeField] MuseumBuildObserver BuildObserver;

    private void OnTriggerEnter(Collider other)
    {
        BuildObserver.TeleportIfPossible();
    }

}
