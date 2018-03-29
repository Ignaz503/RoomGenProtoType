using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetadataController : MonoBehaviour {

    public Camera PlayerCam;
    Ray r;
	// Use this for initialization
	void Start () {
        //PlayerCam = Camera.main;
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 screenPoint = PlayerCam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        r = PlayerCam.ScreenPointToRay(screenPoint);
        RaycastHit hit;
        if (Physics.Raycast(r, out hit, 7.5f, LayerMask.GetMask(new string[] {"Display"})))
        {
            Display disp = hit.transform.gameObject.GetComponent<Display>();
            if (disp != null)
                Debug.Log(disp.Metadata);
        }
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(r);
    }

}
