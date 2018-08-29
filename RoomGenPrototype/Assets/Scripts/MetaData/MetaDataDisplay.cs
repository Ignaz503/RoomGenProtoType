using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityStandardAssets.Characters.FirstPerson;


/// <summary>
/// The display for all the metadata
/// TODO: make interactable and define pickup interaction
/// </summary>
public class MetaDataDisplay : MonoBehaviour, IHoldableObject
{
    /// <summary>
    /// The canvas that holds all the buttons that are lead to the singular displays
    /// </summary>
    [SerializeField]Canvas buttonBaseCanvas;

    /// <summary>
    /// the canvas that holds all the pages displaying the different information
    /// </summary>
    [SerializeField] Canvas displayBaseCanvas;

    /// <summary>
    /// the prefab for the metadata button that when clicked opens the page for this info
    /// </summary>
    [SerializeField] GameObject metadataButtonsPrefab;

    /// <summary>
    /// the prefab for the singular pages displaying the information
    /// </summary>
    [SerializeField] GameObject displayPagePrefab;

    /// <summary>
    /// Containers for the tmp text fields associated with what information of the metadata they need to display(taken from metadata attribute)
    /// </summary>
    List<Tuple<TextMeshProUGUI,string>> contentDisplays = new List<Tuple<TextMeshProUGUI, string>>();

    /// <summary>
    /// first person controller of player used to disable/enable cursor lock
    /// </summary>
    FirstPersonController fpc;

    /// <summary>
    /// The local position of the gameobject when placed in a hand
    /// </summary>
    [SerializeField] Vector3 inHandPosition;
    
    /// <summary>
    /// the rotation of the displa when in rest position 
    /// </summary>
    [SerializeField] Vector3 inRestPositionRotation;

    /// <summary>
    /// rotation of display when held infront of face
    /// </summary>
    [SerializeField] Vector3 infontOfFaceRotation;

    /// <summary>
    /// how many seconds it should take to rotate from one rotation to another
    /// </summary>
    [SerializeField] float secondsToRotate;

    /// <summary>
    /// velocity used by smoothdamp to rotate
    /// </summary>
    Vector3 vel = Vector3.zero;


    /// <summary>
    /// the gameobject the behaviour is placed upon(used by interface)
    /// </summary>
    public GameObject Object
    {
        get
        {
            return gameObject;
        }
    }

    // Use this for initialization
    void Awake () {
        Type t = typeof(MetaData);
        PropertyInfo[] fields = t.GetProperties(); ;

        SetupButtonPrefabCorrectly(fields.Length);

        foreach (PropertyInfo field in fields)
        {

            MetaDataAttribute att = field.GetCustomAttribute<MetaDataAttribute>() as MetaDataAttribute;
            if (att != null)
            {
                CreateMetadataDisplay(att);
            }// end if att != null

        }// end foreach
        displayBaseCanvas.gameObject.SetActive(false);
	}

    /// <summary>
    /// setsup layout element preferred height for easier more correct 
    /// button set up
    /// </summary>
    /// <param name="numFields">the number of fields the metadata has</param>
    void SetupButtonPrefabCorrectly(int numFields)
    {
        LayoutElement elem =  metadataButtonsPrefab.GetComponent<LayoutElement>();
        if(elem == null)
        {
            throw new Exception("MetadataButtonsPrefab needs component layoutelement");
        }
        elem.preferredHeight = 1.0f / numFields;
    }

    /// <summary>
    /// creates the metadata display for a certain attribute
    /// </summary>
    /// <param name="attribute"></param>
    void CreateMetadataDisplay(MetaDataAttribute attribute)
    {
        GameObject btnObj = Instantiate(metadataButtonsPrefab);
        
        btnObj.transform.SetParent(buttonBaseCanvas.transform);
        btnObj.transform.localPosition = Vector3.zero;
        btnObj.transform.localEulerAngles = Vector3.zero;
        btnObj.name = attribute.Type+ " Btn";
        TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
        btnText.text = attribute.Type;

        //Instantiate display for this button
        GameObject metaDisp = Instantiate(displayPagePrefab);
        metaDisp.transform.SetParent(displayBaseCanvas.transform);
        metaDisp.transform.localPosition = Vector3.zero;
        metaDisp.transform.localEulerAngles = Vector3.zero;
        metaDisp.name = attribute.Type + " Page";
        Button backbutton = metaDisp.GetComponentInChildren<Button>();
        if(backbutton.name != "BackButton")
        {
            Debug.LogError("this is an error");
        }
        backbutton.onClick.AddListener(() => 
        {
            //deactivate self, activate buttons base
            //deactivate display base
            metaDisp.SetActive(false);
            buttonBaseCanvas.gameObject.SetActive(true);
            displayBaseCanvas.gameObject.SetActive(false);
        });

        Button metaButton = btnObj.GetComponent<Button>();
        metaButton.onClick.AddListener(() => {
            // set button base canvas inactive
            // set dsiplay base canvas active
            // set meta disp associated to this button active
            buttonBaseCanvas.gameObject.SetActive(false);
            displayBaseCanvas.gameObject.SetActive(true);
            metaDisp.SetActive(true);
        });

        //remember content textmeshProUGUI
        TextMeshProUGUI text = metaDisp.transform.GetChild(1).transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            text.name = attribute.Type;
            contentDisplays.Add(new Tuple<TextMeshProUGUI, string>(text, attribute.Type));
        }
        else
            Debug.Log("Fuck you haha");
        metaDisp.SetActive(false);
    }

    /// <summary>
    /// Applys a metadata to the display
    /// </summary>
    /// <param name="meta">the metadata whose information should be displayed</param>
    void ApplyMetadataToDislplay(MetaData meta)
    {

        foreach(Tuple<TextMeshProUGUI,string> contDisp in contentDisplays)
        {
            string text_to_apply = "";
            if(contDisp.Item2 == "FurtherInfo")
            {
                //TODO IMPLEMENT PROFICENCY
                text_to_apply = meta.GetFieldWithAttributeTypeAsString(contDisp.Item2, 1);
            }
            else
            {
                text_to_apply = meta.GetFieldWithAttributeTypeAsString(contDisp.Item2);
            }
            contDisp.Item1.text = text_to_apply;

            contDisp.Item1.ForceMeshUpdate();
            (contDisp.Item1.rectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                contDisp.Item1.GetRenderedValues().y);
            Debug.Log(contDisp.Item1.text);
            Debug.Log(contDisp.Item1.name);

        }// end foreach
    }

    /// <summary>
    /// stops all corutines sets local rotation
    /// dissolves cursor lock
    /// </summary>
    public void OnHeldInfrontOfFace()
    {
        transform.localEulerAngles = Vector3.zero;
        StopAllCoroutines();
        fpc.SetCursorLock(false);
    }

    /// <summary>
    /// sets inactive and stops all corutines
    /// </summary>
    public void OnPutAway()
    {
        transform.localEulerAngles = inRestPositionRotation;
        gameObject.SetActive(false);
        StopAllCoroutines();
    }

    /// <summary>
    /// starts corutine to rotate to 0deg and sets self active
    /// </summary>
    public void OnStartedMoveInfrontOfFace()
    {
        gameObject.SetActive(true);
        //TODO start corutine that rotates from -90 degree to 0
        StopAllCoroutines();
        StartCoroutine(RotateTo(infontOfFaceRotation));
    }

    /// <summary>
    /// start corutine to rotate towards 90 deg and locks cursor
    /// </summary>
    public void OnStartedMoveAwayFromFace()
    {
        //TODO set cursor lock of first person controller to true;
        fpc.SetCursorLock(true);
        StopAllCoroutines();
        StartCoroutine(RotateTo(inRestPositionRotation));
    }

    /// <summary>
    /// sets local position to somthing depending on hand state
    /// </summary>
    /// <param name="hand">the hand holding the display</param>
    public void PositionObjectInHand(PlayerHand hand)
    {
        //TODO set local position to somthing or so
        transform.localPosition = inHandPosition;
    }

    /// <summary>
    /// is intersted in interactable objects should probably be a property
    /// </summary>
    /// <returns>true</returns>
    public bool IsInterestedInInteractableObjects()
    {
        return true;
    }

    /// <summary>
    /// applies the metadata of the interacted object if it is a display that has been interacted with
    /// </summary>
    /// <param name="iObj">the interacted object</param>
    public void SetInteractedObject(IInteractable iObj)
    {
        Display d = iObj.Object.GetComponent<Display>();
        if (d != null && d.MetaData != null)
        {
            ApplyMetadataToDislplay(d.MetaData);
        }
    }

    /// <summary>
    /// calls poisiton object in hand and gets the fpc of the player
    /// </summary>
    /// <param name="hand"></param>
    public void OnGrabed(PlayerHand hand)
    {
        fpc = hand.OwningPlayer.FirstPersonController;
        PositionObjectInHand(hand);
    }

    /// <summary>
    /// stops all corutines and clears the fpc
    /// clears metadata probably
    /// and sets cursor lock
    /// </summary>
    /// <param name="hand"></param>
    public void OnDropped(PlayerHand hand)
    {
        StopAllCoroutines();
        //TODO
        fpc = null;
        //clear metadata
        //set cursor lock to true
        //position it at 0
    }

    /// <summary>
    /// rotates to given rotation in local coords
    /// </summary>
    /// <param name="locEuler">local angle to rotate towards</param>
    IEnumerator RotateTo(Vector3 locEuler)
    {
        while ((transform.localEulerAngles - locEuler).magnitude >= 1)
        {
            transform.localEulerAngles = Vector3.SmoothDamp(transform.localEulerAngles, locEuler, ref vel, secondsToRotate);
            yield return null;
        }
        Debug.Log("Done rotating");
        vel = Vector3.zero;
    }

}
