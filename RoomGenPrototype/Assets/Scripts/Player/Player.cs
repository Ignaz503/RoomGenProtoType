using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour {

    public Camera PlayerCam;
    Ray CenterRay;
    [SerializeField][Range(0.5f,100f)] float rayCastMaxDist;
    [SerializeField] KeyCode interactKey;
    [SerializeField] GameObject interactionPrompt;
	// Use this for initialization
	void Start () {
        //PlayerCam = Camera.main;

        TextMeshProUGUI prompt = interactionPrompt.GetComponentInChildren<TextMeshProUGUI>();
        prompt.text = $"Press {interactKey} to itneract";

    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 screenPoint = PlayerCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        CenterRay = PlayerCam.ScreenPointToRay(screenPoint);
        RaycastHit hit;
        if (Physics.Raycast(CenterRay, out hit, rayCastMaxDist, LayerMask.GetMask(new string[] { "Display"})))
        {
            //TODO: MAKE SURE NO WALL BETWEEN PLAYER AND DISPLAY
            Display disp = hit.transform.gameObject.GetComponentInChildren<Display>();
            if (disp != null)
            {
                //Debug.Log(disp.GetMetadata());

                //set prompt position and visibility
                ActivateInteractionPromt(disp.transform.position);

            }// end if disp null
            else
            {
                //disable prompt
                interactionPrompt.SetActive(false);
                disp = null;
            }
        }// end if raycast hit
	}

    void ActivateInteractionPromt(Vector3 displayPosition)
    {
        Vector3 promptPos = Vector3.Lerp(PlayerCam.transform.position, displayPosition, 0.5f);

        interactionPrompt.transform.position = promptPos;
        interactionPrompt.transform.rotation = PlayerCam.transform.rotation;
        interactionPrompt.SetActive(true);
    }
}
