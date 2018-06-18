using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionPrompt : MonoBehaviour {

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Player player;
    [SerializeField] GameObject obj;
    private void Start()
    {

        if(player != null)
        {
            player.OnInInteractionRange += (args) => 
            {
                PlaceBetweenAndRotateTowardsFirst(args.PlayerCamera.transform, args.ObjectTransform, .5f); obj = args.ObjectInteractedWith; player = args.InteractingPlayer; SetActive(true);
            };

            player.OnOutOfInteractionRange += () => { SetActive(false); };

            player.OnInteractionStart += (disp) => { SetActive(false); };
            player.OnInteractionEnd += (disp) =>
            {
                if ((player.transform.position - disp.ObjectTransform.position).magnitude <= player.RayCastMaxDist)
                {
                    SetActive(true);
                }
            };
        }
        else
        {
            Debug.Log("No Player was found");
        }

        SetActive(false);
    }

    private void LateUpdate()
    {
        PlaceBetweenAndRotateTowardsFirst(player.PlayerCam.transform, obj.transform, .5f);
        Debug.Log(text.textBounds);
    }

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
        text.ForceMeshUpdate();
    }

}
