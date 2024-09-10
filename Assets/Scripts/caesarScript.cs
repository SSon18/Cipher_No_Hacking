using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PuzzleSystem : MonoBehaviour
{
    // UI References for Puzzle
    [SerializeField]
    private GameObject puzzleCanvas;

    [SerializeField]
    private TMP_Text ciphertextText;

    [SerializeField]
    private TMP_InputField answerInputField;

    [SerializeField]
    private Button submitButton;

    [SerializeField]
    private TMP_Text feedbackText;

    [SerializeField]
    private TMP_Text timerText; // Reference to the timer text UI element

    [SerializeField]
    private TMP_Text missionText;
    [SerializeField]
    [TextArea]
    private string[] missionWords;

    // Puzzle Configuration
    [SerializeField]
    private int shift = 3; // Shift for Caesar Cipher
    [SerializeField]
    private string plaintext = "Antivirus"; // Original text

    private string ciphertext;
    private string previousCiphertext; // Store the previous ciphertext
    private bool puzzleActivated;

    // Timer Configuration
    [SerializeField]
    private float timeLimit = 30.0f; // Time limit in seconds
    private float timer;
    private bool timerRunning;

    // Possible plaintexts to randomize when time runs out
    [SerializeField]
    private string[] possiblePlaintexts;

    // Flag to check if the puzzle has been solved
    private bool puzzleSolved = false;

    // Reference to the next level door (ensure it's assigned in the Inspector)
    [SerializeField]
    private GameObject nextLevelDoor; // Object for the door

    // Reference to Player Movement Script (Ensure the player is assigned in the Inspector)
    [SerializeField]
    private NewBehaviourScript playerMovementScript;

    private void Start()
    {
        // Encrypt the plaintext to generate ciphertext
        ciphertext = Encrypt(plaintext, shift);
        previousCiphertext = ciphertext; // Initialize previousCiphertext

        // Set up initial UI states
        puzzleCanvas.SetActive(false);
        submitButton.onClick.AddListener(OnSubmit);

        // Initialize mission text with the first mission
        if (missionWords.Length > 0)
        {
            DisplayMissionText(missionWords[0]);
        }
    }

    private void  Update()
    {
        // Check if the timer is running and update it
        if (timerRunning)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0; // Ensure timer doesn't go below zero
                timerRunning = false;
                ChangeCiphertext();
            }

            // Update the timer display
            UpdateTimerDisplay();
        }
    }

    public void ActivatePuzzle()
    {
        if (!puzzleSolved)
        {
            puzzleCanvas.SetActive(true);
            ciphertextText.text = "Ciphertext: " + ciphertext;

            // Start the timer
            timer = timeLimit;
            timerRunning = true;

            // Ensure input field is interactable and visible
            answerInputField.interactable = true;
            answerInputField.gameObject.SetActive(true);

            // Optionally, set focus to the input field
            answerInputField.Select();
            answerInputField.ActivateInputField(); // Allows typing

            // Disable player movement
            if (playerMovementScript != null)
            {
                playerMovementScript.enabled = false;
            }
        }
    }

    public void OnSubmit()
    {
        string userAnswer = answerInputField.text.Trim(); // Trim spaces

        Debug.Log("Submitted answer: " + userAnswer);
        Debug.Log("Expected plaintext: " + plaintext);

        if (userAnswer.Equals(plaintext, System.StringComparison.OrdinalIgnoreCase))
        {
            DisplayFeedback("Correct!");
            puzzleSolved = true; // Mark the puzzle as solved
            timerRunning = false; // Stop the timer
            puzzleCanvas.SetActive(false); // Hide the puzzle UI

            // Enable player movement after puzzle is solved
            if (playerMovementScript != null)
            {
                playerMovementScript.enabled = true;
            }

            // Enable the nextLevelDoor's trigger
            if (nextLevelDoor != null)
            {
                Collider2D doorCollider = nextLevelDoor.GetComponent<Collider2D>();

                if (doorCollider != null)
                {
                    doorCollider.enabled = true;
                    doorCollider.isTrigger = true; // Ensure the collider is a trigger
                    Debug.Log("nextLevelDoor collider enabled and set as trigger.");
                }
                else
                {
                    Debug.LogWarning("No Collider2D found on nextLevelDoor.");
                }
            }
            else
            {
                Debug.LogWarning("nextLevelDoor reference is missing.");
            }
        }
        else
        {
            DisplayFeedback("Try again!");
        }
    }

    private void ChangeCiphertext()
    {
        // Store the current ciphertext as previous
        previousCiphertext = ciphertext;

        // Ensure a new ciphertext is different from the previous one
        string newPlaintext;
        do
        {
            newPlaintext = possiblePlaintexts[Random.Range(0, possiblePlaintexts.Length)];
            ciphertext = Encrypt(newPlaintext, shift);
        } while (ciphertext == previousCiphertext);

        // Update the plaintext variable to reflect the new value
        plaintext = newPlaintext;

        ciphertextText.text = "Ciphertext: " + ciphertext;

        // Optioqnally, reset the timer for the new ciphertext
        timer = timeLimit;
        timerRunning = true;

        // Update the mission text based on a new element or logic
        DisplayMissionText(missionWords[Random.Range(0, missionWords.Length)]);

        // Debugging
        Debug.Log("New plaintext: " + plaintext);
        Debug.Log("New ciphertext: " + ciphertext);
    }

    private void DisplayFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
    }

    private void DisplayMissionText(string message)
    {
        if (missionText != null)
        {
            missionText.text = message;
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = "Time Left: " + Mathf.Ceil(timer).ToString(); // Display time rounded up
        }
    }

    private string Encrypt(string text, int shift)
    {
        char[] buffer = new char[text.Length];
        for (int i = 0; i < text.Length; i++)
        {
            char letter = text[i];
            if (char.IsLetter(letter))
            {
                char d = char.IsUpper(letter) ? 'A' : 'a';
                buffer[i] = (char)((((letter + shift) - d) % 26) + d);
            }
            else
            {
                buffer[i] = letter;
            }
        }
        return new string(buffer);
    }
}
