using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoBehaviour {

    public Camera PlayerCam;
    Ray CenterRay;
    [SerializeField][Range(0.5f,100f)] float rayCastMaxDist;
    [SerializeField] KeyCode interactKey;
    [SerializeField] InteractionPrompt interactionPrompt;
    [SerializeField] FirstPersonController firstPersonController;

    Display lastDisplayHit;
	// Use this for initialization
	void Start () {
        //PlayerCam = Camera.main;
        interactionPrompt.SetText($"Press {interactKey} to itneract");
    }
	
	// Update is called once per frame
	void Update ()
    {
        HandleDisplayRaycast();

        HandleInteractionPrompt();

        HandleInteraction();
 
	}

    void HandleDisplayRaycast()
    {
        Vector3 screenPoint = PlayerCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        CenterRay = PlayerCam.ScreenPointToRay(screenPoint);
        RaycastHit hit;
        if (Physics.Raycast(CenterRay, out hit, rayCastMaxDist, LayerMask.GetMask(new string[] { "Display" })))
        {
            //TODO: MAKE SURE NO WALL BETWEEN PLAYER AND DISPLAY
            Display disp = hit.transform.gameObject.GetComponentInChildren<Display>();
            if (disp != null)
            {
                lastDisplayHit = disp;

            }// end if disp null
        }// end if raycast hit
        else
        {
            //no hit
            lastDisplayHit = null;
        }
    }

    void HandleInteractionPrompt()
    {
        if(lastDisplayHit == null)
        {
            interactionPrompt.SetActive(false);
        }
        else
        {
            interactionPrompt.PlaceBetweenAndRotateTowardsFirst(PlayerCam.transform, lastDisplayHit.transform, .5f);
            interactionPrompt.SetActive(true);
        }


    }

    void HandleInteraction()
    {
        if (lastDisplayHit != null && Input.GetKeyDown(interactKey))
        {
            //do stuff
            // eg. load scene
            // put display mesh gameobject into "hand"   
            //firstPersonController.enabled = !firstPersonController.enabled;
            if(lastDisplayHit is IInteractable)
            {
                (lastDisplayHit as IInteractable).Interact(this);
            }
        }
    }

}
