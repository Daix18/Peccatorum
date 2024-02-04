using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;

public class DialogueController : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    public float textSpeed;

    private Story currentStory;

    private bool dialogueIsPlaying;
    private bool isPrinting;

    public static DialogueController THIS;
    // Start is called before the first frame update
    void Start()
    {
        THIS = this;
        isPrinting = false;
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogueIsPlaying && !isPrinting)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentStory.canContinue)
                {
                    ContinueStory();
                }
                else
                {
                    ExitDialogueMode();
                }
            }
        }
    }

    public void EnterDialogueMode(TextAsset inkJson)
    {
        currentStory = new Story(inkJson.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            string text = currentStory.Continue();
            dialogueText.text = "";
            StartCoroutine(PrintTextByCharacter(text));
        }
        else
        {
            ExitDialogueMode();
        }
    }

    IEnumerator PrintTextByCharacter(string text)
    {
        isPrinting = true;
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        isPrinting = false;
    }

    public static DialogueController GetInstance()
    {
        return THIS;
    }
}
