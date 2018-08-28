using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Box collider trigger that opens and closes a door it is responsible for
/// </summary>
public class DoorTrigger : MonoBehaviour {

    [SerializeField] BaseDoor door;
    [SerializeField] bool openOnStart = true;

    private void Start()
    {
        if (openOnStart)
        {
            //Temp
            Action<BaseDoor> lockOpen = null;
            lockOpen = (tmepDoor) => { door.LockOpen(); door.OnOpen -= lockOpen; };
            door.OnOpen += lockOpen;

            door.Open();
        }
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
