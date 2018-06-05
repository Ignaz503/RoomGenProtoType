using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Player : MonoBehaviour {

    public enum State
    {
        Moving,
        Interacting
    }

    #region Events
    /// <summary>
    /// called right before IInteracttable function called
    /// and player state set to interacting
    /// same frame as interact button is pressed
    /// </summary>
    public event Action<PlayerInteractionEventArgs> OnInteractionStart;

    /// <summary>
    /// Called right befor playerstate is set to not interacting, the frame where the
    /// cancel interaction button is pressed
    /// </summary>
    public event Action<PlayerInteractionEventArgs> OnInteractionEnd;

    public event Action<PlayerInteractionEventArgs> OnInInteractionRange;
    public event Action OnOutOfInteractionRange;
    //maybe on Interrupt?
    #endregion

    public Camera PlayerCam;
    Ray centerRay;
    [SerializeField][Range(0.5f, 100f)] float rayCastMaxDist;
    public float RayCastMaxDist { get { return rayCastMaxDist; } protected set { rayCastMaxDist = value; } }
    [SerializeField] KeyCode interactKey;
    [SerializeField] InteractionPrompt interactionPrompt;
    public FirstPersonController FirstPersonController;
    [SerializeField] State playerState;

    Display lastDisplayHit;
	// Use this for initialization
	void Start () {
        //PlayerCam = Camera.main;
        playerState = State.Moving;
        interactionPrompt.SetText($"Press {interactKey} to itneract");
    }
	
	// Update is called once per frame
	void Update ()
    {
        HandleDisplayRaycast();

        HandleInteraction();
	}

    void HandleDisplayRaycast()
    {
        if(playerState == State.Moving)
        {
            Vector3 screenPoint = PlayerCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
            centerRay = PlayerCam.ScreenPointToRay(screenPoint);
            RaycastHit[] hits =
            Physics.RaycastAll(centerRay, RayCastMaxDist, LayerMask.GetMask(new string[] { "Wall", "Display" }));
            if(hits.Length >0)
            {
                //check if first hit is display
                if(hits[0].transform.gameObject.layer == LayerMask.NameToLayer("Display"))
                {
                    //TODO: MAKE SURE NO WALL BETWEEN PLAYER AND DISPLAY
                    Display disp = hits[0].transform.gameObject.GetComponentInChildren<Display>();
                    if (disp != null)
                    {
                        lastDisplayHit = disp;
                        if (disp is IInteractable)
                        {
                            OnInInteractionRange?.Invoke(new PlayerDisplayInteractionEventArgs(this, lastDisplayHit));
                        }
                    }// end if disp null
                }//end if 
            }// end if hit more than 0
            else
            {
                //no hit
                if(lastDisplayHit != null)
                {
                    OnOutOfInteractionRange?.Invoke();
                }
                lastDisplayHit = null;
            }
        }
    }

    void HandleInteraction()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if(playerState == State.Interacting)
            {
                OnInteractionEnd?.Invoke(new PlayerDisplayInteractionEventArgs(this, lastDisplayHit));
                playerState = State.Moving;
            } else {
                if(lastDisplayHit != null)
                {
                    //do stuff
                    // eg. load scene
                    // put display mesh gameobject into "hand"   
                    //firstPersonController.enabled = !firstPersonController.enabled;
                    if (lastDisplayHit is IInteractable)
                    {
                        OnInteractionStart?.Invoke(new PlayerDisplayInteractionEventArgs(this, lastDisplayHit));

                        playerState = State.Interacting;
                        (lastDisplayHit as IInteractable).Interact(this);
                    }// end if is IInteractabele
                }// end if lastDispHit != null
            }// end if else state == interacting
        }// end if key down
    }

}
