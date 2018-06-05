using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHand : MonoBehaviour {

    public enum HandState
    {
        RestPosition,
        MovingToRestPosition,
        MovingToInfrontOfFacePosition,
        InfrontOfFace
    }
    [SerializeField] HandState State = HandState.RestPosition;
    [SerializeField] Vector3 holdInfrontOfFacePosition;
    [SerializeField] Vector3 holdInFrontOfFaceRotaition;
    [SerializeField] Vector3 holdInFrontOfFaceScale;
    [SerializeField] KeyCode MoveHandKey = KeyCode.Mouse1;

    Vector3 restPosition;
    Vector3 restRotation;
    Vector3 restScale;

    [SerializeField] Player player;
    [SerializeField] TextMeshProUGUI displayText;

    private void Start()
    {
        restPosition = transform.localPosition;
        restRotation = transform.localEulerAngles;
        restScale = transform.localScale;

        player.OnInInteractionRange += (args) =>
        {
            if (args.InteractionType == PlayerInteractionEventArgs.InteractingWith.Display)
            {
                Debug.Log("Poss interaction with display");
                PlayerDisplayInteractionEventArgs disArgs = args as PlayerDisplayInteractionEventArgs;
                displayText.text = disArgs.DisplayInteractedWith.GetMetadata();
            }
        };

    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(MoveHandKey))
        {
            switch (State)
            {
                case HandState.RestPosition:
                    State = HandState.MovingToInfrontOfFacePosition;
                    break;
                case HandState.MovingToInfrontOfFacePosition:
                    State = HandState.MovingToRestPosition;
                    break;
                case HandState.InfrontOfFace:
                    State = HandState.MovingToRestPosition;
                    break;
                case HandState.MovingToRestPosition:
                    State = HandState.MovingToInfrontOfFacePosition;
                    break;
            }
        }
        DummyAction();
    }

    void DummyAction()
    {
        if(State == HandState.MovingToInfrontOfFacePosition)
        {
            transform.localPosition = holdInfrontOfFacePosition;
            transform.localEulerAngles = holdInFrontOfFaceRotaition;
            transform.localScale = holdInFrontOfFaceScale;
            State = HandState.InfrontOfFace;
        }
        else if(State == HandState.MovingToRestPosition)
        {
            transform.localPosition = restPosition;
            transform.localEulerAngles = restRotation;
            transform.localScale = restScale;
            State = HandState.RestPosition;
        }
        if(State == HandState.InfrontOfFace)
        {
            transform.localPosition = new Vector3(player.PlayerCam.transform.localPosition.x, holdInfrontOfFacePosition.y, holdInfrontOfFacePosition.z);
        }
    }

}
