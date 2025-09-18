using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Dialogue : MonoBehaviour
{
    [Header("Dialogue Components")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public string[] dialogue;
    private int index;

    public GameObject contButton;
    public float wordSpeed;
    private bool playerIsClose;

    [Header("Player Control")]
    public GameObject player; // Reference to player object
    private PlayerController playerMovement; // Script reference for player movement

    [Header("Ending Choices")]
    public GameObject choicePanel; // Panel for the choices
    public Button choice1Button;
    public Button choice2Button;
    public Button choice3Button;

    [Header("Auto Dialogue Trigger")]
    public bool autoTrigger; // Will trigger the dialogue automatically
    private bool dialogueTriggered;

    void Start()
    {
        playerMovement = player.GetComponent<PlayerController>();
        dialogueTriggered = false;

        // Hide choice panel at the start
        choicePanel.SetActive(false);

        // Add listener for the continue button to trigger NextLine()
        contButton.GetComponent<Button>().onClick.AddListener(NextLine);

        // Assign buttons for endings
        choice1Button.onClick.AddListener(() => ChooseEnding("Ending1"));
        choice2Button.onClick.AddListener(() => ChooseEnding("Ending2"));
        choice3Button.onClick.AddListener(() => ChooseEnding("Ending3"));
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
        choicePanel.SetActive(true);
        EnablePlayerControl();
    }

    void ChooseEnding(string ending)
    {
        Debug.Log("Player chose: " + ending);
        SceneManager.LoadScene(ending); // Load the respective ending scene
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsClose = true;

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
