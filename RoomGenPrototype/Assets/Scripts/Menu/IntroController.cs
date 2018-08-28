using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroController : MonoBehaviour {

    MuseumRequestData request = new MuseumRequestData()
    {
        MuseumType = "Random"
    };

    [SerializeField] List<GameObject> invlovedPanels;

    private void Awake()
    {
        //set to first element visible
        Enabel(0);
    }

    public void Enabel(int idx)
    {
        foreach (GameObject obj in invlovedPanels)
            obj.SetActive(false);
        invlovedPanels[idx].SetActive(true);
    }

    public void SetSize(int size)
    {
        request.Size = (MuseumSize)size;
        invlovedPanels[3].GetComponentsInChildren<TextMeshProUGUI>()[0].text = $"You chose {request.Size.ToString().ToLower()}...";
    }

    public void RequestMuseum()
    {
        MuseumRequest req = new MuseumRequest(request, (m) =>
        {
            ResourceLoader.Instance.MuseumToBuild = m;
            SceneManager.LoadScene("_Museum_");
        });
        ResourceLoader.Instance.PostRequest(req, ResourceLoader.RequestType.MuseumRequest);
    }

}
