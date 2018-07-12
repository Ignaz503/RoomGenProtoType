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

    /// <summary>
    /// called once for any display when the player moves in range to interact with it
    /// WARNING: CURRENTLY CALLED EVERY FRAME UNTIL TODO IS DONE
    /// </summary>
    public event Action<PlayerInteractionEventArgs> OnInInteractionRange;

 /// <summary>
    /// called the frame when player moves out of interaction range when the interacable object in the previous fame was not null
    /// </summary>
    public event System.Action OnOutOfInteractionRange;
    //maybe on Interrupt?
    #endregion

    /// <summary>
    /// The camera that is the player head so to speek
    /// </summary>
    public Camera PlayerCam;

    /// <summary>
    /// ray that is cent throught the center of the screen to see if anything interactable is hit
    /// </summary>
    Ray centerRay;

    /// <summary>
    /// limits the distance the ray that checks for anything interactbale hit
    /// </summary>
    [SerializeField][Range(0.5f, 100f)] float rayCastMaxDist;
    public float RayCastMaxDist { get { return rayCastMaxDist; } protected set { rayCastMaxDist = value; } }

    /// <summary>
    /// the key that needs to be pressed to start an interaction if possible
    /// </summary>
    [SerializeField] KeyCode interactKey;

    /// <summary>
    /// the prompt that appear when an interaction is possible
    /// should probably be removed
    /// </summary>
    [SerializeField] InteractionPrompt interactionPrompt;

    /// <summary>
    /// The first person controller of the std unity assets
    /// </summary>
    public FirstPersonController FirstPersonController;

    /// <summary>
    /// The state of the player
    /// </summary>
    [SerializeField] State playerState;

    /// <summary>
    /// The player hand
    /// </summary>
    public  PlayerHand PHand;

    /// <summary>
    /// The display that was hit last, can be null
    /// should probaly be IInteractable
    /// </summary>
    IInteractable lastInteractableHit;

    private void Awake()
    {
        PHand.OwningPlayer = this;
    }

    // Use this for initialization
    void Start () {
        //PlayerCam = Camera.main;
       
        playerState = State.Moving;

        if(PHand is PCPlayerHand)
        {
            PCPlayerHand pcHand = PHand as PCPlayerHand;
            interactionPrompt.SetText($"Press {interactKey} to itneract\nPress {pcHand.MoveHandKey} to get out the Information Display");
        }
        else
        {
            //TODO vr hand tracking
            interactionPrompt.SetText($"Press {interactKey} to itneract");
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
        HandleDisplayRaycast();

        HandleInteraction();
	}

    /// <summary>
    /// handles the raycast to see if we can interact with anything
    /// and calls appropriate evetns
    /// </summary>
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
                        //TODO make sure that display is not same as last frame
                        //before calling events
                        lastInteractableHit = disp;
                        if (disp is IInteractable)
                        {
                            OnInInteractionRange?.Invoke(new PlayerDisplayInteractionEventArgs(this, disp));
                        }
                    }// end if disp null
                }//end if 
            }// end if hit more than 0
            else
            {
                //no hit
                if(lastInteractableHit != null)
                {
                    OnOutOfInteractionRange?.Invoke();
                }
                lastInteractableHit = null;
            }
        }
    }

    /// <summary>
    /// handles the possibility of the player interacting with somthing
    /// if he is in range and calls appropriate events
    /// </summary>
    void HandleInteraction()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if(playerState == State.Interacting)
            {
                //TODO interactions events shouldn't focus ondisplays
                OnInteractionEnd?.Invoke(new PlayerDisplayInteractionEventArgs(this, lastInteractableHit as Display));
                playerState = State.Moving;
            } else {
                if(lastInteractableHit != null)
                {
                    //do stuff
                    // eg. load scene
                    // put display mesh gameobject into "hand"   
                    //firstPersonController.enabled = !firstPersonController.enabled;

                    OnInteractionStart?.Invoke(new PlayerDisplayInteractionEventArgs(this, lastInteractableHit as Display));

                    playerState = State.Interacting;
                    lastInteractableHit.Interact(this);
                }// end if lastDispHit != null
            }// end if else state == interacting
        }// end if key down
    }

}
