using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PuzzleSystem : MonoBehaviour
{
    // SCRIPT FOR CAESAR CIPHER!!!

    // UI References for Puzzle
    [SerializeField]
    public GameObject puzzleCanvas; // The main canvas for the puzzle UI

    [SerializeField]
    private TMP_Text ciphertextText; // Text display for the ciphertext

    [SerializeField]
    public TMP_InputField answerInputField; // Input field for user answers

    [SerializeField]
    private Button submitButton; // Button to submit the answer

    [SerializeField]
    public TMP_Text feedbackText; // Text display for feedback messages

    [SerializeField]
    private TMP_Text timerText; // Text display for the countdown timer

    [SerializeField]
    private TMP_Text missionText; // Text display for mission instructions

    [SerializeField]
    [TextArea]
    private string[] missionWords; // Array of words for the mission text

    // Puzzle Configuration
    [SerializeField]
    private int shift = 3; // Shift for Caesar Cipher

    // Use a dictionary to store plaintext and description pairs
    private Dictionary<string, string> plaintextDescriptions;

    private string plaintext; // The current plaintext to solve
    private string ciphertext; // The current ciphertext displayed
    private string previousCiphertext; // Store the previous ciphertext for reference
    private bool puzzleActivated; // Indicates if the puzzle is activated
    private int currentPlaintextIndex; // Index for the current plaintext

    // Timer Configuration
    [SerializeField]
    private float timeLimit = 30.0f; // Time limit for the puzzle
    private float timer; // Timer variable
    private bool timerRunning; // Indicates if the timer is running

    // Possible plaintexts and their descriptions
    [SerializeField]
    private string[] possiblePlaintexts; // Array of potential plaintexts

    [SerializeField]
    private string[] descriptions; // Array of descriptions for plaintexts

    // Correct answer UI panel
    [SerializeField]
    private GameObject correctAnswerPanel; // Panel to show the correct answer

    [SerializeField]
    private TMP_Text correctAnswerWordText; // Text display for the correct answer word

    [SerializeField]
    [TextArea]
    private TMP_Text correctAnswerDescriptionText; // Text display for the correct answer description

    [SerializeField]
    private Button proceedButton; // Reference to the Proceed button

    private bool puzzleSolved = false; // Indicates if the puzzle has been solved

    // Player Movement Script
    [SerializeField]
    private NewBehaviourScript playerMovementScript; // Script for controlling player movement

    // Reference to the next level door (ensure it's assigned in the Inspector)
    [SerializeField]
    private GameObject nextLevelDoor; // Object for the door leading to the next level

    private List<DecipheredWord> decipheredWords = new List<DecipheredWord>(); // List of deciphered words

    // Reference to DialogueSystem
    [SerializeField]
    private DialogueSystem dialogueSystem; // Reference to the dialogue system

    // Class to represent a deciphered word with its description
    [System.Serializable]
    public class DecipheredWord
    {
        public string word; // The deciphered word
        public string description; // Description of the deciphered word

        public DecipheredWord(string word, string description)
        {
            this.word = word; // Initialize the word
            this.description = description; // Initialize the description
        }
    }

    private void Start()
    {
        // Set up the dictionary of plaintext and descriptions
        SetupPlaintextDescriptions();

        // Randomize the first plaintext and its corresponding ciphertext
        RandomizePlaintext();

        // Set up initial UI states
        puzzleCanvas.SetActive(false); // Hide the puzzle canvas at the start
        submitButton.onClick.AddListener(OnSubmit); // Add listener for submit button
        proceedButton.onClick.AddListener(OnProceed); // Add listener for Proceed button

        // Ensure DialogueSystem is assigned correctly
        if (dialogueSystem == null)
        {
            dialogueSystem = FindObjectOfType<DialogueSystem>();
            if (dialogueSystem == null)
            {
                Debug.LogError("DialogueSystem not found in the scene!");
            }
        }
    }

    private void Update()
    {
        // Update the timer if it's running
        if (timerRunning)
        {
            timer -= Time.deltaTime; // Decrease timer by the time elapsed since last frame
            if (timer <= 0)
            {
                timer = 0; // Reset timer to zero if it runs out
                timerRunning = false; // Stop the timer
                ChangeCiphertext(); // Change the ciphertext if the time is up
            }
            UpdateTimerDisplay(); // Update the timer display UI
        }
    }

    // Pair plaintexts and descriptions in a dictionary
    private void SetupPlaintextDescriptions()
    {
        plaintextDescriptions = new Dictionary<string, string>(); // Initialize dictionary

        // Ensure that possiblePlaintexts and descriptions arrays have the same length
        if (possiblePlaintexts.Length != descriptions.Length)
        {
            Debug.LogError("Possible plaintexts and descriptions arrays must have the same length.");
            return; // Exit the method if lengths don't match
        }

        // Populate the dictionary with plaintext-description pairs
        for (int i = 0; i < possiblePlaintexts.Length; i++)
        {
            plaintextDescriptions.Add(possiblePlaintexts[i], descriptions[i]);
        }

        // Debug log to confirm the setup
        Debug.Log("Plaintext Descriptions Set Up:");
        foreach (var item in plaintextDescriptions)
        {
            Debug.Log($"Word: {item.Key}, Description: {item.Value}"); // Log each pair
        }
    }

    private void RandomizePlaintext()
    {
        // Reset the plaintextDescriptions to ensure it's fresh for each puzzle
        SetupPlaintextDescriptions();

        // Randomly select a plaintext and encrypt it to create a ciphertext
        currentPlaintextIndex = Random.Range(0, possiblePlaintexts.Length);
        plaintext = possiblePlaintexts[currentPlaintextIndex];
        ciphertext = Encrypt(plaintext, shift); // Encrypt the selected plaintext

        // Update UI to display the new ciphertext
        ciphertextText.text = "Ciphertext: " + ciphertext;
    }

    public void ActivatePuzzle()
    {
        // Activate the puzzle if it has not been solved
        if (!puzzleSolved)
        {
            puzzleActivated = true; // Set this to true to indicate the puzzle is active
            SetupPlaintextDescriptions(); // Ensure the descriptions are reset for the new puzzle
            puzzleCanvas.SetActive(true); // Show the puzzle canvas
            ciphertextText.text = "Ciphertext: " + ciphertext; // Display the ciphertext
            timer = timeLimit; // Set the timer to the limit
            timerRunning = true; // Start the timer

            // Setup input field
            answerInputField.interactable = true; // Enable input field
            answerInputField.gameObject.SetActive(true); // Activate the input field
            answerInputField.Select(); // Select the input field
            answerInputField.ActivateInputField(); // Activate the input field for user input

            // Disable player movement
            if (playerMovementScript != null)
            {
                playerMovementScript.enabled = false; // Disable player movement
            }

            // Display mission text when the puzzle begins
            DisplayMissionText(); // Show the mission text
        }
    }



    private void DisplayMissionText()
    {
        // Display the mission text based on the mission words
        if (missionText != null && missionWords.Length > 0)
        {
            // For example, displaying the first mission word
            missionText.text = "Mission: " + missionWords[0];
        }
    }

    public void OnSubmit()
    {
        // Handle answer submission from the input field
        string userAnswer = answerInputField.text.Trim(); // Trim any whitespace

        // Check if the user's answer matches the plaintext
        if (userAnswer.Equals(plaintext, System.StringComparison.OrdinalIgnoreCase))
        {
            DisplayFeedback("Correct!"); // Show correct feedback
            puzzleSolved = true; // Mark the puzzle as solved
            timerRunning = false; // Stop the timer
            puzzleCanvas.SetActive(false); // Hide the puzzle canvas

            // Check if the word is already in the list to avoid duplicates
            if (!decipheredWords.Exists(dw => dw.word.Equals(plaintext, System.StringComparison.OrdinalIgnoreCase)))
            {
                decipheredWords.Add(new DecipheredWord(plaintext, plaintextDescriptions[plaintext])); // Store the solved word
                SaveDecipheredWords(); // Save deciphered words after solving the puzzle
            }

            // Show the correct answer panel with the plaintext
            ShowCorrectAnswerPanel(plaintext); // Ensure this is passing the correct plaintext
            EnableNextLevelDoorTrigger(); // Enable the next level door trigger
        }
        else
        {
            DisplayFeedback("Try again!"); // Show incorrect feedback
        }
    }


    private void EnableNextLevelDoorTrigger()
    {
        // Enable the next level door trigger
        if (nextLevelDoor != null)
        {
            BoxCollider2D doorCollider = nextLevelDoor.GetComponent<BoxCollider2D>(); // Get the door's collider

            if (doorCollider != null)
            {
                doorCollider.isTrigger = true; // Ensure the collider is a trigger
                Debug.Log("nextLevelDoor collider enabled and set as trigger."); // Log success
            }
            else
            {
                Debug.LogWarning("No BoxCollider2D found on nextLevelDoor."); // Log warning if no collider
            }
        }
        else
        {
            Debug.LogWarning("nextLevelDoor reference is missing."); // Log warning if door reference is missing
        }
    }

    private void ChangeCiphertext()
    {
        // Change the current ciphertext when the timer runs out
        previousCiphertext = ciphertext; // Store the current ciphertext
        RandomizePlaintext(); // Randomize the next plaintext and ciphertext

        // Update the UI with the new ciphertext
        ciphertextText.text = "Ciphertext: " + ciphertext;
        timer = timeLimit; // Reset the timer
        timerRunning = true; // Start the timer again
    }

    private void ResetCorrectAnswerPanel()
    {
        // Reset the correct answer panel for reuse
        if (correctAnswerPanel == null)
        {
            Debug.LogError("correctAnswerPanel is not assigned!");
            return; // Exit the method if the panel is null
        }

        if (correctAnswerWordText == null)
        {
            Debug.LogError("correctAnswerWordText is not assigned!");
            return; // Exit the method if the text is null
        }

        if (correctAnswerDescriptionText == null)
        {
            Debug.LogError("correctAnswerDescriptionText is not assigned!");
            return; // Exit the method if the text is null
        }

        // Clear the text and hide the panel
        correctAnswerWordText.text = ""; // Clear the text
        correctAnswerDescriptionText.text = ""; // Clear the description
        correctAnswerPanel.SetActive(false); // Hide the panel initially
    }

    private void ShowCorrectAnswerPanel(string decipheredWord)
    {
        // Show the correct answer panel with the deciphered word
        ResetCorrectAnswerPanel(); // Reset the panel before displaying new content

        if (correctAnswerPanel != null && correctAnswerWordText != null && correctAnswerDescriptionText != null)
        {
            string description = "";
            if (plaintextDescriptions.TryGetValue(decipheredWord, out description))
            {
                // Set the text for the correct answer
                correctAnswerWordText.text = "Deciphered Word: " + decipheredWord;
                correctAnswerDescriptionText.text = description; // Set the description
            }
            else
            {
                correctAnswerWordText.text = "Deciphered Word: " + decipheredWord;
                correctAnswerDescriptionText.text = "No description available."; // Handle missing description
                Debug.LogWarning("No description available for: " + decipheredWord); // Log warning
            }

            correctAnswerPanel.SetActive(true); // Show the panel with updated content
        }
        else
        {
            Debug.LogError("Correct answer panel or text fields are not set up properly!"); // Log error if setup is incorrect
        }
    }

    private void DisplayFeedback(string message)
    {
        // Display feedback message to the user
        if (feedbackText != null)
        {
            feedbackText.text = message; // Update the feedback text
        }
    }

    private void UpdateTimerDisplay()
    {
        // Update the timer display on the UI
        if (timerText != null)
        {
            timerText.text = "Time Left: " + Mathf.Ceil(timer).ToString(); // Show the remaining time
        }
    }

    private string Encrypt(string text, int shift)
    {
        // Encrypt the plaintext using the Caesar cipher
        char[] buffer = new char[text.Length]; // Create a buffer for the encrypted characters
        for (int i = 0; i < text.Length; i++)
        {
            char letter = text[i]; // Get the current character
            if (char.IsLetter(letter)) // Check if it's a letter
            {
                char d = char.IsUpper(letter) ? 'A' : 'a'; // Determine base for shifting
                buffer[i] = (char)((((letter + shift) - d) % 26) + d); // Encrypt the letter
            }
            else
            {
                buffer[i] = letter; // Non-letter characters remain unchanged
            }
        }
        return new string(buffer); // Return the encrypted string
    }

    // Function to handle the Proceed button click
    public void OnProceed()
    {
        // Hide the correct answer panel when proceeding
        if (correctAnswerPanel != null)
        {
            correctAnswerPanel.SetActive(false);
        }

        // Re-enable player movement when proceeding
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true; // Enable player movement
        }

        // Reset puzzle activation state
        puzzleActivated = false; // Set this back to false to indicate the puzzle is no longer active

        // Optionally, reset dialogue for the next puzzle
        if (dialogueSystem != null)
        {
            dialogueSystem.ResetDialogue(); // Reset the dialogue state
        }
    }


    // When viewing the album, ensure that words are saved properly
    public void OnViewAlbumButtonClick()
    {
        // Ensure the words are saved before switching scenes
        SaveDecipheredWords();

        // Load the Album scene
        SceneManager.LoadScene(8); // Switch to the album scene
    }

    // This method saves the deciphered words to PlayerPrefs
    private void SaveDecipheredWords()
    {
        // Clear existing deciphered words from PlayerPrefs
        LoadDecipheredWords();

        // Convert the updated deciphered words list to JSON and store in PlayerPrefs
        string json = JsonUtility.ToJson(new WordsContainer { words = new List<DecipheredWord>(new HashSet<DecipheredWord>(decipheredWords)) }); // Remove duplicates
        PlayerPrefs.SetString("DecipheredWords", json); // Save to PlayerPrefs
        PlayerPrefs.Save(); // Ensure data is written

        Debug.Log("Deciphered words saved: " + json); // Log saved words
    }


    // This method loads existing deciphered words from PlayerPrefs and updates the list
    private void LoadDecipheredWords()
    {
        // Check if deciphered words are already saved in PlayerPrefs
        if (PlayerPrefs.HasKey("DecipheredWords"))
        {
            string json = PlayerPrefs.GetString("DecipheredWords"); // Get the saved JSON
            WordsContainer loadedWords = JsonUtility.FromJson<WordsContainer>(json); // Deserialize the JSON

            if (loadedWords != null && loadedWords.words != null)
            {
                foreach (var word in loadedWords.words)
                {
                    // Only add the word if it doesn't already exist in the list
                    if (!decipheredWords.Exists(dw => dw.word.Equals(word.word, System.StringComparison.OrdinalIgnoreCase)))
                    {
                        decipheredWords.Add(word); // Add loaded words to the list
                    }
                }
            }
        }
    }


    // Helper class to serialize the list of words
    [System.Serializable]
    private class WordsContainer
    {
        public List<DecipheredWord> words; // List of deciphered words
    }
}
