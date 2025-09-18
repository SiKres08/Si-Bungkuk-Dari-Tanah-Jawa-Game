using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Dialogue2 : MonoBehaviour
{
    [Header("Dialogue Components")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public string[] dialogue;
    private int index;

    public GameObject contButton;
    public float wordSpeed;
    private bool playerIsClose;
    public bool playerHasTriggeredDialogue { get; private set; }

    [Header("Player Control")]
    public GameObject player; // Reference to player object
    private PlayerController playerMovement; // Script reference for player movement

    [Header("Auto Dialogue Trigger")]
    public bool autoTrigger; // Will trigger the dialogue automatically
    private bool dialogueTriggered;

    void Start()
    {
        playerMovement = player.GetComponent<PlayerController>();
        dialogueTriggered = false;

        // Add listener for the continue button to trigger NextLine()
        contButton.GetComponent<Button>().onClick.AddListener(NextLine);
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && playerIsClose && !autoTrigger)
        {
            TriggerDialogue();
        }

        if (dialogueText.text == dialogue[index])
        {
            contButton.SetActive(true);
        }
    }

    void TriggerDialogue()
    {
        if (dialoguePanel.activeInHierarchy)
        {
            zeroText();
        }
        else
        {
            dialoguePanel.SetActive(true);
            StartCoroutine(Typing());
            DisablePlayerControl();
        }
    }

    public void zeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
        EnablePlayerControl();
    }

    IEnumerator Typing()
    {
        foreach (char letter in dialogue[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
        contButton.SetActive(true);
    }

    public void NextLine()
    {
        Debug.Log("Next Line Button Clicked");

        contButton.SetActive(false);

        if (index < dialogue.Length - 0)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            DisplayChoices();
        }
    }

    private void DisplayChoices()
    {
        dialoguePanel.SetActive(false);
        EnablePlayerControl();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;
            playerHasTriggeredDialogue = true;

            if (autoTrigger && !dialogueTriggered)
            {
                TriggerDialogue();
                dialogueTriggered = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = false;
            zeroText();
        }
    }

    private void DisablePlayerControl()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    private void EnablePlayerControl()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }
}
