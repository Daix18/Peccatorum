using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    //El texto que contará el diálogo.
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private string[] lines;
    [SerializeField] private float testSpeed;
    int index;

    Image image;

    // Start is called before the first frame update
    void Start()
    {
        dialogueText.text = string.Empty;

        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(index);
        if (NPController.THIS.playerIsClose == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartDialogue();
            }

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log(index);
                if (dialogueText.text == lines[index])
                {
                    NextLine();
                }
                else
                {
                    StopAllCoroutines();
                    dialogueText.text = lines[index];
                }
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        image.enabled = true;
        dialogueText.text = string.Empty;
        dialogueText.gameObject.SetActive(true);
        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            dialogueText.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            image.enabled = false;
            dialogueText.gameObject.SetActive(false);
        }
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(testSpeed);
        }
    }
}
