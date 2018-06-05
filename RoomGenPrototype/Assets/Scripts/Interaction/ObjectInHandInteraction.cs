using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[InteractionAttribute(typeof(ActivatorSideInteractionContainer))]
public class ObjectInHandInteraction : Interaction
{
    Player playerHoldingObject;
    Transform objectToRotate;
    [SerializeField] KeyCode rotationKey = KeyCode.Mouse0;

    Vector3 initCamLocPos;
    float currentT = 0f;
    Vector3 vel;

    private void Update()
    {
        HandleRotation();
        HandleZoom();
    }

    void HandleRotation()
    {
        if (Input.GetKey(rotationKey))
        {
            objectToRotate.RotateRelativeToCamera(playerHoldingObject.PlayerCam, Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), RotateRelative.World);
        }
    }

    void HandleZoom()
    {
        float zoom = Input.GetAxis("Mouse ScrollWheel");

        if(zoom != 0.0f)
        {
            currentT += zoom;
            currentT = Mathf.Clamp(currentT, 0f, .9f);
            Vector3 newPos =Vector3.Lerp(
                playerHoldingObject.transform.position + initCamLocPos, objectToRotate.position,
                  currentT);

            playerHoldingObject.PlayerCam.transform.position = Vector3.SmoothDamp(playerHoldingObject.PlayerCam.transform.position, newPos, ref vel, Time.deltaTime);
        }
    }

    /// <summary>
    /// sets the object and player that are itneracting
    /// </summary>
    public void StartInteraction(Player p, Transform objToRot)
    {
        playerHoldingObject = p;
        objectToRotate = objToRot;
        initCamLocPos = p.PlayerCam.transform.localPosition;
        playerHoldingObject.FirstPersonController.enabled = false;
    }

    private void OnDestroy()
    {
        playerHoldingObject.FirstPersonController.enabled = true;
    }

    public override void StartInteraction(GameObject activator, GameObject interactedUpon)
    {
        StartInteraction(activator.GetComponent<Player>(), interactedUpon.GetComponent<MeshDisplay>().ChildMesh.transform);
    }
}