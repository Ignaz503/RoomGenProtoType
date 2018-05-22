using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionPrompt : MonoBehaviour {

    [SerializeField] TextMeshProUGUI text;

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    public void PlaceAtAndRotate(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
    
    public void PlaceAt(Vector3 positon)
    {
        transform.position = positon;
    }

    public void SetRotation(Quaternion rot)
    {
        transform.rotation = rot;
    }

    public void PlaceBetween(Vector3 pos1, Vector3 pos2, float t)
    {
        transform.position = Vector3.Lerp(pos1, pos2, t);
    }
    
    public void PlaceBetweenAndRotateTowardsFirst(Transform first,Transform second,float t)
    {
        PlaceBetween(first.position, second.position, t);
        transform.rotation = first.rotation;
    }

    public void SetText(string new_text)
    {
        text.text = new_text;
    }

}
