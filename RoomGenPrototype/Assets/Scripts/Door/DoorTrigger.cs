using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour {

    [SerializeField] BaseDoor door;

    private void OnTriggerEnter(Collider other)
    {
        door.Open();
    }

    private void OnTriggerExit(Collider other)
    {
        door.Close();
    }


}
