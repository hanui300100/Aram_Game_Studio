using UnityEngine;
using TMPro;
using System.Collections;

public class NpcInteraction : MonoBehaviour
{
    public GameObject npc;
    public GameObject dialogueBox;
    public TextMeshPro dialogueText;
    public string[] dialogue;
    private int currentDialogueIndex = 0;
    private string text;
    private float delay = 0.12f;
    private bool isDialogueActive = false;

    void Start()
    {
        text = dialogue[currentDialogueIndex].ToString();
        dialogueText.text = " ";
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isDialogueActive = true;
            StartCoroutine(textPrint(delay));
            dialogueBox.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isDialogueActive = false;
            dialogueBox.SetActive(false);
            EndDialogue();        }
    }

    void Update()
    {
        if (dialogueBox.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                DisplayDialogue();
            }
        }
    }

    void DisplayDialogue()
    {
        if (currentDialogueIndex < dialogue.Length)
        {
            NextDialogue();
        }
    }

    void NextDialogue()
    {
        currentDialogueIndex++;
        if (currentDialogueIndex < dialogue.Length)
        {
            text = dialogue[currentDialogueIndex];
            StartCoroutine(textPrint(delay));
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogueBox.SetActive(false);
        currentDialogueIndex = 0;
        text = dialogue[currentDialogueIndex];
        dialogueText.text = " ";
    }

    IEnumerator textPrint(float d)
    {
            foreach (char c in text)
            {
                if (isDialogueActive)
                {
                    dialogueText.text += c;
                    yield return new WaitForSeconds(d);
                }
                else
                {
                    break;
                }
            }
    }
}