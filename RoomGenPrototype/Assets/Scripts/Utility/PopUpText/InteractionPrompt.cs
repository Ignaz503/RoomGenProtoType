using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionPrompt : MonoBehaviour {

    /// <summary>
    /// text field where text should be displayed
    /// </summary>
    [SerializeField] TextMeshProUGUI text;
    /// <summary>
    /// the player for to whom the text should appear infront of
    /// </summary>
    [SerializeField] Player player;

    /// <summary>
    /// the gamobject where between the player and it, the pop up text should appear
    /// </summary>
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
    }
    
    /// <summary>
    /// sets gamobject active or inactive
    /// </summary>
    /// <param name="state">the active state</param>
    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    /// <summary>
    /// sets location and rotation of gamobject(world position)
    /// </summary>
    /// <param name="position">the world position of the gameobject</param>
    /// <param name="rotation">the world rotation of the gameobject</param>
    public void PlaceAtAndRotate(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
    }
    
    /// <summary>
    /// sets world potsition of gameobject
    /// </summary>
    /// <param name="positon">the world position</param>
    public void PlaceAt(Vector3 positon)
    {
        transform.position = positon;
    }

    /// <summary>
    /// sets world rotation of gameobject
    /// </summary>
    /// <param name="rot">world rotation</param>
    public void SetRotation(Quaternion rot)
    {
        transform.rotation = rot;
    }

    /// <summary>
    /// places gamobject between two world positions with linear interpolation
    /// </summary>
    /// <param name="pos1">first position in world coords</param>
    /// <param name="pos2">second position in world coords</param>
    /// <param name="t">time param of linear interpolation</param>
    public void PlaceBetween(Vector3 pos1, Vector3 pos2, float t)
    {
        transform.position = Vector3.Lerp(pos1, pos2, t);
    }

    /// <summary>
    /// Places gamobject between two other gamobjects and rotates it so that it looks
    /// at the first param
    /// </summary>
    /// <param name="first">transform of gObj which should be looked at</param>
    /// <param name="second">transform of gObj where it should be placed between with first</param>
    /// <param name="t">time param of linear interpolation</param>
    public void PlaceBetweenAndRotateTowardsFirst(Transform first,Transform second,float t)
    {
        PlaceBetween(first.position, second.position, t);
        transform.rotation = first.rotation;
    }

    /// <summary>
    /// sets the text and forces a mesh update of the TMPro object
    /// </summary>
    /// <param name="new_text">the new text that should be displayed</param>
    public void SetText(string new_text)
    {
        text.text = new_text;
        text.ForceMeshUpdate();
    }

}
