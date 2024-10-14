using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class keywordDialogue : MonoBehaviour
{
    // UI References for Dialogue
    [SerializeField]
    private GameObject dialogueCanvas;

    [SerializeField]
    private TMP_Text speakerText;

    [SerializeField]
    private TMP_Text dialogueText;

    [SerializeField]
    private Image portraitImage;

    // Dialogue Content
    [SerializeField]
    private string[] speaker;

    [SerializeField]
    [TextArea]
    private string[] dialogueWords;

    [SerializeField]
    private Sprite[] portrait;

    private bool dialogueActivated;
    private int step;
    private bool isDisplaying; // New flag to check if displaying dialogue

    // Reference to the Puzzle System
    [SerializeField]
    private KeywordCipherSystem keyWord;

    private void Start()
    {
        // Set up initial UI states
        dialogueCanvas.SetActive(false);

        // Ensure PuzzleSystem is assigned correctly
        if (keyWord == null)
        {
            keyWord = FindObjectOfType<KeywordCipherSystem>();

            if (keyWord == null)
            {
                Debug.LogError("PuzzleSystem not found in the scene!");
            }
        }
    }

    private void Update()
    {
        // Check for input to trigger dialogue
     
        // Prevent dialogue interaction if the game is paused (Time.timeScale == 0)
        if (Time.timeScale == 0)
            return;

        // Check if the puzzle is currently active to prevent "Talk" input from interfering
        if (keyWord != null && keyWord.keywordPuzzleCanvas.activeSelf)
        {
            return; // If the puzzle is active, exit the method and don't process "Talk" input
        }

        // Handle "Talk" button press when the puzzle is NOT active
        if (Input.GetButtonDown("Talk") && dialogueActivated && !isDisplaying) // Check if not displaying
        {
            if (step >= speaker.Length) // Check if dialogue is over
            {
                dialogueCanvas.SetActive(false);

                // Activate puzzle system once dialogue ends
                keyWord.ActivatePuzzle();

            }
            else
            {
                dialogueCanvas.SetActive(true);
                speakerText.text = speaker[step];
                StartCoroutine(DisplayDialogue(dialogueWords[step])); // Start letter-by-letter display
                portraitImage.sprite = portrait[step];
                step++;

                // Clear input field and feedback when advancing the dialogue
                keyWord.answerInputField.text = ""; // Clear input field
                keyWord.feedbackText.text = ""; // Clear any previous feedback
            }
        }
    }

    private IEnumerator DisplayDialogue(string dialogue)
    {
        isDisplaying = true; // Set the flag to true when starting the display
        dialogueText.text = ""; // Clear previous text

        foreach (char letter in dialogue)
        {
            dialogueText.text += letter; // Add one letter at a time
            yield return new WaitForSeconds(0.05f); // Adjust the speed here (0.05 seconds per letter)
        }

        isDisplaying = false; // Set the flag to false when finished displaying
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || Input.GetButtonDown("Talk"))
        {
            dialogueActivated = true; // Activate dialogue when entering the collider
            Debug.Log("Player entered the dialogue trigger zone.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            dialogueActivated = false; // Deactivate dialogue when leaving the collider
            Debug.Log("Player exited the dialogue trigger zone.");

            // Deactivate dialogueCanvas if the player is no longer in the trigger zone
            if (dialogueCanvas != null && dialogueCanvas.activeInHierarchy)
            {
                dialogueCanvas.SetActive(false);
                step = 0; // Reset step or dialogue progress
            }
        }
    }

    // Method to reset dialogue state (to be called when puzzle is complete)
    public void ResetDialogue()
    {
        step = 0; // Reset the step counter for new dialogue
        dialogueActivated = false; // Deactivate dialogue to allow re-triggering
        dialogueCanvas.SetActive(false); // Hide the dialogue canvas
    }

    // Method to handle actions after puzzle is completed
    public void OnPuzzleComplete()
    {
        // Reset the dialogue for future interactions
        ResetDialogue();

        // If any other actions are required when puzzle completes, add them here
        Debug.Log("Puzzle completed and dialogue reset.");
    }
}
