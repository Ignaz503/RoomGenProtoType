using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Box collider trigger that opens and closes a door it is responsible for
/// </summary>
public class DoorTrigger : MonoBehaviour {

    [SerializeField] BaseDoor door;

    private void Start()
    {
        //Temp
        Action<BaseDoor> lockOpen = null;
        lockOpen = (tmepDoor) => { door.LockOpen(); door.OnOpen -= lockOpen; };
        door.OnOpen += lockOpen;

        door.Open();
    }


    private void OnTriggerEnter(Collider other)
    {
        door.Open();
    }

    private void OnTriggerExit(Collider other)
    {
        door.Close();
    }


}
