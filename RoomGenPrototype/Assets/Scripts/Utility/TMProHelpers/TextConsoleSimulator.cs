using UnityEngine;
using System.Collections;
using TMPro;

public class TextConsoleSimulator : MonoBehaviour
{
    private TMP_Text m_TextComponent;
    private bool hasTextChanged;
    [SerializeField] GameObject[] EnableOnDone;
    [SerializeField] MonoBehaviour[] EnableBehavioursOnDone;
    [SerializeField]float WaitForSeconds;
    [SerializeField] bool PerWordReveal = false;

    void Awake()
    {
        m_TextComponent = gameObject.GetComponent<TMP_Text>();
        WaitForSeconds = Mathf.Max(0f, WaitForSeconds); //make positive
    }

    void Start()
    {
        if(!PerWordReveal)
            StartCoroutine(RevealCharacters(m_TextComponent));
        else
            StartCoroutine(RevealWords(m_TextComponent));
    }

    private void Update()
    {
        if(Input.anyKeyDown)
        {
            StopAllCoroutines();
            m_TextComponent.maxVisibleCharacters = m_TextComponent.textInfo.characterCount;
            RevaeleGameObjects();
            EnableBehaviours();
        }
    }

    void OnEnable()
    {
        // Subscribe to event fired when text object has been regenerated.
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
    }

    void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
    }


    // Event received when the text object has changed.
    void ON_TEXT_CHANGED(Object obj)
    {
        hasTextChanged = true;
    }

    void RevaeleGameObjects()
    {
        foreach (GameObject obj in EnableOnDone)
            obj.SetActive(true);
    }

    void EnableBehaviours()
    {
        foreach (MonoBehaviour bh in EnableBehavioursOnDone)
            bh.enabled = true;
    }

    /// <summary>
    /// Method revealing the text one character at a time.
    /// </summary>
    /// <returns></returns>
    IEnumerator RevealCharacters(TMP_Text textComponent)
    {
        textComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = textComponent.textInfo;

        int totalVisibleCharacters = textInfo.characterCount; // Get # of Visible Character in text object
        int visibleCount = 0;

        while (visibleCount <= totalVisibleCharacters)
        {
            if (hasTextChanged)
            {
                totalVisibleCharacters = textInfo.characterCount; // Update visible character count.
                hasTextChanged = false;
            }

            //if (visibleCount > totalVisibleCharacters)
            //{
            //    yield return new WaitForSeconds(1.0f);
            //    visibleCount = 0;
            //}

            textComponent.maxVisibleCharacters = visibleCount; // How many characters should TextMeshPro display?

            visibleCount += 1;

            if (WaitForSeconds != 0f)
                yield return new WaitForSeconds(WaitForSeconds);
            else
                yield return null;
        }
        RevaeleGameObjects();
        EnableBehaviours();
    }


    /// <summary>
    /// Method revealing the text one word at a time.
    /// </summary>
    /// <returns></returns>
    IEnumerator RevealWords(TMP_Text textComponent)
    {
        textComponent.ForceMeshUpdate();

        int totalWordCount = textComponent.textInfo.wordCount;
        int totalVisibleCharacters = textComponent.textInfo.characterCount; // Get # of Visible Character in text object
        int counter = 0;
        int currentWord = 0;
        int visibleCount = 0;

        while (visibleCount < totalVisibleCharacters)
        {
            Debug.Log($"{totalVisibleCharacters}: {visibleCount}");
            currentWord = counter % (totalWordCount + 1);

            // Get last character index for the current word.
            if (currentWord == 0) // Display no words.
                visibleCount = 0;
            else if (currentWord < totalWordCount) // Display all other words with the exception of the last one.
                visibleCount = textComponent.textInfo.wordInfo[currentWord - 1].lastCharacterIndex + 1;
            else if (currentWord == totalWordCount) // Display last word and all remaining characters.
                visibleCount = totalVisibleCharacters;

            textComponent.maxVisibleCharacters = visibleCount; // How many characters should TextMeshPro display?

            counter += 1;
            if (WaitForSeconds != 0f)
                yield return new WaitForSeconds(WaitForSeconds);
            else
                yield return null;
        }
        RevaeleGameObjects();
        EnableBehaviours();
    }

}
