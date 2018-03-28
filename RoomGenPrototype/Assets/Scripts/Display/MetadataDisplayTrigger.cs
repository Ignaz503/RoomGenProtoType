using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetadataDisplayTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Come on and slam, and welcome to the jam");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("bye bye female dog");
    }
}
