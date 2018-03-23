using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetadataDisplay : MonoBehaviour {

    public static MetadataDisplay Instance;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void SetActiveTo(bool value)
    {
        Debug.Log(value);
        gameObject.SetActive(value);
    }
}
