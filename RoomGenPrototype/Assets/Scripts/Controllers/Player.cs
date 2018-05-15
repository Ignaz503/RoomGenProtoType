using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour {

    public Camera PlayerCam;
    Ray CenterRay;
    [SerializeField][Range(0.5f,100f)] float RayCastMaxDist;
    [SerializeField] KeyCode interactKey;
	// Use this for initialization
	void Start () {
        //PlayerCam = Camera.main;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 screenPoint = PlayerCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        CenterRay = PlayerCam.ScreenPointToRay(screenPoint);
        RaycastHit hit;
        if (Physics.Raycast(CenterRay, out hit, RayCastMaxDist, LayerMask.GetMask(new string[] {"Display"})))
        {
            Display disp = hit.transform.gameObject.GetComponent<Display>();
            if (disp != null)
            {
                Debug.Log(disp.GetMetadata());
            }// end if disp null
        }// end if raycast hit
	}

}
