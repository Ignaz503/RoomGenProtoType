using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MetaDataDisplay : MonoBehaviour {

    [SerializeField]Canvas buttonBaseCanvas;
    [SerializeField] Canvas displayBaseCanvas;
    [SerializeField] GameObject metadataButtonsPrefab;
    [SerializeField] GameObject displayPagePrefab;
    List<Tuple<TextMeshProUGUI,string>> contentDisplays = new List<Tuple<TextMeshProUGUI, string>>();
	// Use this for initialization
	void Start () {

        Type t = typeof(MetaData);
        FieldInfo[] fields = t.GetFields();

        SetupButtonPrefabCorrectly(fields.Length);

        foreach (FieldInfo field in fields)
        {
            MetaDataAttribute att = field.GetCustomAttribute<MetaDataAttribute>() as MetaDataAttribute;
            if (att != null)
            {
                //Debug.Log(field.Name);
                CreateMetadataDisplay(att);
            }// end if att != null
        }// end foreach
        displayBaseCanvas.gameObject.SetActive(false);

	}

    void SetupButtonPrefabCorrectly(int numFields)
    {
        LayoutElement elem =  metadataButtonsPrefab.GetComponent<LayoutElement>();
        if(elem == null)
        {
            throw new Exception("MetadataButtonsPrefab needs component layoutelement");
        }
        elem.preferredHeight = 1.0f / numFields;
    }

    bool test = true;
    void CreateMetadataDisplay(MetaDataAttribute attribute)
    {
        GameObject btnObj = Instantiate(metadataButtonsPrefab);
        
        btnObj.transform.SetParent(buttonBaseCanvas.transform);
        btnObj.transform.localPosition = Vector3.zero;
        btnObj.name = attribute.Type+ " Btn";
        TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
        btnText.text = attribute.Type;

        //Instantiate display for this button
        GameObject metaDisp = Instantiate(displayPagePrefab);
        metaDisp.transform.SetParent(displayBaseCanvas.transform);
        metaDisp.transform.localPosition = Vector3.zero;
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
        TextMeshProUGUI[] texts = metaDisp.GetComponentsInChildren<TextMeshProUGUI>();
        foreach(TextMeshProUGUI text in texts)
        {
            if(text.gameObject.name == "Content")
            {
                contentDisplays.Add(new Tuple<TextMeshProUGUI, string>(text,attribute.Type));

                if(test)
                {
                    test = false;
                }
            }
        }

        metaDisp.SetActive(false);
    }
	// Update is called once per frame
	void Update () {
		
	}

    void ApplyMetadataToDislplay(MetaData meta)
    {
        foreach(Tuple<TextMeshProUGUI,string> contDisp in contentDisplays)
        {
            float height = 0f;
            string text_to_apply = "";
            if(contDisp.Item2 == "FurtherInfo")
            {
                //TODO IMPLEMENT PROFICENCY
                text_to_apply = meta.GetFieldWithAttributeTypeAsString(contDisp.Item2, 1);
                height = contDisp.Item1.GetPreferredValues(text_to_apply).y;
            }
            else
            {
                text_to_apply = meta.GetFieldWithAttributeTypeAsString(contDisp.Item2);
                height = contDisp.Item1.GetPreferredValues(text_to_apply).y;
            }
            contDisp.Item1.text = text_to_apply;
            contDisp.Item1.ForceMeshUpdate();
            (contDisp.Item1.rectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                contDisp.Item1.GetRenderedValues().y);

        }// end foreach
    }

}
