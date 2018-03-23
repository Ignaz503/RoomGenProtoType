using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetadataDisplayTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        MetadataDisplay.Instance.SetActiveTo(true);
    }

    private void OnTriggerExit(Collider other)
    {
        MetadataDisplay.Instance.SetActiveTo(false);
    }
}
