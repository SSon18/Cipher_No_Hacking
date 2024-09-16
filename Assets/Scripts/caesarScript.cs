using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PuzzleSystem : MonoBehaviour 
{
    // SCRIPT FOR CAESAR CIPHER!!!
    // SCRIPT FOR CAESAR CIPHER!!!
    // SCRIPT FOR CAESAR CIPHER!!!

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
    private TMP_Text timerText;

    [SerializeField]
    private TMP_Text missionText;

    [SerializeField]
    [TextArea]
    private string[] missionWords;

    // Puzzle Configuration
    [SerializeField]
    private int shift = 3; // Shift for Caesar Cipher

    // Use a dictionary to store plaintext and description pairs
    private Dictionary<string, string> plaintextDescriptions;

    private string plaintext;
    private string ciphertext;
    private string previousCiphertext;
    private bool puzzleActivated;
    private int currentPlaintextIndex;

    // Timer Configuration
    [SerializeField]
    private float timeLimit = 30.0f;
    private float timer;
    private bool timerRunning;

    // Possible plaintexts and their descriptions
    [SerializeField]
    private string[] possiblePlaintexts;

    [SerializeField]
    private string[] descriptions;

    // Correct answer UI panel
    [SerializeField]
    private GameObject correctAnswerPanel;

    [SerializeField]
    private TMP_Text correctAnswerWordText;

    [SerializeField]
    [TextArea]
    private TMP_Text correctAnswerDescriptionText;

    [SerializeField]
    private Button proceedButton; // Reference to the Proceed button

    private bool puzzleSolved = false;

    // Player Movement Script
    [SerializeField]
    private NewBehaviourScript playerMovementScript;

    // Reference to the next level door (ensure it's assigned in the Inspector)
    [SerializeField]
    private GameObject nextLevelDoor; // Object for the door

    private void Start()
    {
        // Set up the dictionary of plaintext and descriptions
        SetupPlaintextDescriptions();

        // Randomize the first plaintext and its corresponding ciphertext
        RandomizePlaintext();

        // Set up initial UI states
        puzzleCanvas.SetActive(false);
        submitButton.onClick.AddListener(OnSubmit);
        proceedButton.onClick.AddListener(OnProceed); // Add listener for Proceed button
    }

    private void Update()
    {
        if (timerRunning)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0;
                timerRunning = false;
                ChangeCiphertext();
            }
            UpdateTimerDisplay();
        }
    }

    // Pair plaintexts and descriptions in a dictionary
    private void SetupPlaintextDescriptions()
    {
        plaintextDescriptions = new Dictionary<string, string>();

        // Ensure that possiblePlaintexts and descriptions arrays have the same length
        if (possiblePlaintexts.Length != descriptions.Length)
        {
            Debug.LogError("Possible plaintexts and descriptions arrays must have the same length.");
            return;
        }

        for (int i = 0; i < possiblePlaintexts.Length; i++)
        {
            plaintextDescriptions.Add(possiblePlaintexts[i], descriptions[i]);
        }
    }

    private void RandomizePlaintext()
    {
        currentPlaintextIndex = Random.Range(0, possiblePlaintexts.Length);
        plaintext = possiblePlaintexts[currentPlaintextIndex];
        ciphertext = Encrypt(plaintext, shift);

        // Update UI
        ciphertextText.text = "Ciphertext: " + ciphertext;
    }

    public void ActivatePuzzle()
    {
        if (!puzzleSolved)
        {
            puzzleCanvas.SetActive(true);
            ciphertextText.text = "Ciphertext: " + ciphertext;

            timer = timeLimit;
            timerRunning = true;

            answerInputField.interactable = true;
            answerInputField.gameObject.SetActive(true);
            answerInputField.Select();
            answerInputField.ActivateInputField();

            if (playerMovementScript != null)
            {
                playerMovementScript.enabled = false; // Disable player movement
            }

            // Display mission text when the puzzle begins
            DisplayMissionText();
        }
    }

    private void DisplayMissionText()
    {
        if (missionText != null && missionWords.Length > 0)
        {
            // For example, displaying the first mission word
            missionText.text = "Mission: " + missionWords[0];
        }
    }

    public void OnSubmit()
    {
        string userAnswer = answerInputField.text.Trim();

        if (userAnswer.Equals(plaintext, System.StringComparison.OrdinalIgnoreCase))
        {
            DisplayFeedback("Correct!");
            puzzleSolved = true;
            timerRunning = false;
            puzzleCanvas.SetActive(false);

            ShowCorrectAnswerPanel(plaintext);

            EnableNextLevelDoorTrigger();
        }
        else
        {
            DisplayFeedback("Try again!");
        }
    }

    private void EnableNextLevelDoorTrigger()
    {
        if (nextLevelDoor != null)
        {
            BoxCollider2D doorCollider = nextLevelDoor.GetComponent<BoxCollider2D>();

            if (doorCollider != null)
            {
                doorCollider.isTrigger = true; // Ensure the collider is a trigger
                Debug.Log("nextLevelDoor collider enabled and set as trigger.");
            }
            else
            {
                Debug.LogWarning("No BoxCollider2D found on nextLevelDoor.");
            }
        }
        else
        {
            Debug.LogWarning("nextLevelDoor reference is missing.");
        }
    }

    private void ChangeCiphertext()
    {
        previousCiphertext = ciphertext;
        RandomizePlaintext();

        ciphertextText.text = "Ciphertext: " + ciphertext;
        timer = timeLimit;
        timerRunning = true;
    }

    private void ShowCorrectAnswerPanel(string decipheredWord)
    {
        if (correctAnswerPanel != null && correctAnswerWordText != null && correctAnswerDescriptionText != null)
        {
            string description = "";
            if (plaintextDescriptions.TryGetValue(decipheredWord, out description))
            {
                correctAnswerWordText.text = "Deciphered Word: " + decipheredWord;
                correctAnswerDescriptionText.text = description;
            }
            else
            {
                correctAnswerWordText.text = "Deciphered Word: " + decipheredWord;
                correctAnswerDescriptionText.text = "No description available.";
                Debug.LogWarning("No description available for: " + decipheredWord);
            }

            correctAnswerPanel.SetActive(true);
        }
    }

    private void DisplayFeedback(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = "Time Left: " + Mathf.Ceil(timer).ToString();
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

    // Function to handle the Proceed button click
    public void OnProceed()
    {
        if (correctAnswerPanel != null)
        {
            correctAnswerPanel.SetActive(false);
        }

        // Re-enable player movement when proceeding
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
    }
}
