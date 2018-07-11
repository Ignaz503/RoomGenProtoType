using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// TODO FIX
/// </summary>
public class ObejctInHandInteraction : Interaction
{
    /// <summary>
    /// the component behaviour when an object is in hand and rotated
    /// </summary>
    ObjectInHandComponent comp;
 
    public override void StartInteraction(GameObject activator, GameObject interactedUpon)
    {
        comp = activator.AddComponent<ObjectInHandComponent>();
        comp.StartInteraction(activator, interactedUpon);
    }

    public override void EndInteraction()
    {
        Destroy(comp);
    }
}

/// <summary>
/// the bahaviour that allows to rotate an obnject that is held with the mouse
/// </summary>
public class ObjectInHandComponent : InteractionComponent
{
    /// <summary>
    /// the player holding the object
    /// </summary>
    Player playerHoldingObject;

    /// <summary>
    /// transform of the object that need rotating
    /// </summary>
    Transform objectToRotate;

    /// <summary>
    /// key that need pressing to rotate the object
    /// </summary>
    [SerializeField] KeyCode rotationKey = KeyCode.Mouse0;

    /// <summary>
    /// the initial camera position for zoom reasons
    /// </summary>
    Vector3 initCamLocPos;

    /// <summary>
    /// current T for lerping of zoom level
    /// </summary>
    float currentT = 0f;

    /// <summary>
    /// velocity vector used by smooth damp
    /// </summary>
    Vector3 vel;

    /// <summary>
    /// The Rotation of the object when the interaction starts
    /// </summary>
    Quaternion initialObjectRotation;

    private void Update()
    {
        HandleRotation();
        HandleZoom();
    }

    /// <summary>
    /// Handles rotation of the object
    /// </summary>
    void HandleRotation()
    {
        if (Input.GetKey(rotationKey))
        {
            objectToRotate.RotateRelativeToCamera(playerHoldingObject.PlayerCam, Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), RotateRelative.World);
        }
    }

    /// <summary>
    /// handles the zoom of camera
    /// </summary>
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

    /// <summary>
    /// cleanup funciton after interaction was ended
    /// </summary>
    protected override void Destroy()
    {
        objectToRotate.transform.localRotation = initialObjectRotation;
        playerHoldingObject.FirstPersonController.enabled = true;
    }

    /// <summary>
    /// setup function when interaction starts
    /// </summary>
    /// <param name="activator">obj activating interaction</param>
    /// <param name="interactedUpon">object that is interacted upon</param>
    public override void StartInteraction(GameObject activator, GameObject interactedUpon)
    {
        StartInteraction(activator.GetComponent<Player>(), interactedUpon.GetComponent<MeshDisplay>().ChildMesh.transform);
    }
}