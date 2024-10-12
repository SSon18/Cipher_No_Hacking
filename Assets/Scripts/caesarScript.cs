using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Transactions;
using System;

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

    [SerializeField]
    public GameObject albumPanel;
    [SerializeField]
    public BoxCollider2D puzzleCollider; // Assuming you are using a BoxCollider2D

    private SpriteRenderer notifRenderer;
    private GameObject notif;
    [SerializeField]
    private GameObject interactable;
    // Class to represent a deciphered word with its description
    [System.Serializable]
    public class DecipheredWord : IEquatable<DecipheredWord>
    {
        public string word;
        public string description;

        public DecipheredWord(string word, string description)
        {
            this.word = word;
            this.description = description;
        }

        // Implement Equals and GetHashCode to compare words properly
        public bool Equals(DecipheredWord other)
        {
            if (other == null) return false;
            return this.word.Equals(other.word, System.StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return word.ToLower().GetHashCode();
        }
    }
    private SpriteRenderer doorSpriteRenderer; // Declare a SpriteRenderer reference

    private void Start()
    {
        LoadPuzzleState(); // PUZZLE SAVER !!!!!!!!!!!!!!!!!
        LoadDecipheredWords();
        
        doorSpriteRenderer = nextLevelDoor.GetComponent<SpriteRenderer>();
        if (doorSpriteRenderer != null)
        {
            doorSpriteRenderer.enabled = false; // Hide the sprite initially
        }
        else
        {
            Debug.LogWarning("No SpriteRenderer found on nextLevelDoor during Start."); // Log warning if missing
        }

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
    private void SavePuzzleState()
    {
        // Unique key for this puzzle
        string puzzleKey = "PuzzleSolved_" + gameObject.name;

        // Save the puzzle solved state
        PlayerPrefs.SetInt(puzzleKey, puzzleSolved ? 1 : 0);

        // Save the next level door state (enabling isTrigger)
        if (nextLevelDoor != null)
        {
            string doorKey = "NextLevelDoor_" + nextLevelDoor.name;
            bool doorTriggerEnabled = nextLevelDoor.GetComponent<Collider2D>().isTrigger;
            PlayerPrefs.SetInt(doorKey, doorTriggerEnabled ? 1 : 0);
        }

        // Save the interactable state (active/inactive)
        if (interactable != null)
        {
            string interactableKey = "Interactable_" + gameObject.name;
            PlayerPrefs.SetInt(interactableKey, interactable.activeSelf ? 1 : 0); // Save the active state
        }

        PlayerPrefs.Save(); // Ensure everything is saved
    }

    private void LoadPuzzleState()
    {
        // Unique key for this puzzle
        string puzzleKey = "PuzzleSolved_" + gameObject.name;

        // Load the puzzle solved state
        if (PlayerPrefs.HasKey(puzzleKey))
        {
            puzzleSolved = PlayerPrefs.GetInt(puzzleKey) == 1;

            // Disable the collider of this specific puzzle if solved
            if (puzzleSolved && puzzleCollider != null)
            {
                puzzleCollider.enabled = false; // Disable only this puzzle's collider
            }

            // Load and apply the next level door's state (isTrigger)
            if (nextLevelDoor != null)
            {
                string doorKey = "NextLevelDoor_" + nextLevelDoor.name;
                if (PlayerPrefs.HasKey(doorKey))
                {
                    bool doorTriggerEnabled = PlayerPrefs.GetInt(doorKey) == 1;
                    nextLevelDoor.GetComponent<Collider2D>().isTrigger = doorTriggerEnabled;
                }
            }

            // Load and apply the interactable's state
            if (interactable != null)
            {
                string interactableKey = "Interactable_" + gameObject.name;
                if (PlayerPrefs.HasKey(interactableKey))
                {
                    bool interactableActive = PlayerPrefs.GetInt(interactableKey) == 1;
                    interactable.SetActive(interactableActive); // Set the active state from PlayerPrefs
                }
            }

            // Disable dialogue interaction for this puzzle if solved
            if (puzzleSolved && dialogueSystem != null)
            {
                dialogueSystem.enabled = false; // Disable dialogue interaction for this specific puzzle
            }
        }
    }

    public void SolvePuzzle()
    {
        // Set puzzle as solved and save the state
        puzzleSolved = true;

        // Hide the interactable GameObject when the puzzle is solved
        if (interactable != null)
        {
            interactable.SetActive(false); // Hide the interactable
        }

        // Enable the isTrigger on the next level door when the puzzle is solved
        if (nextLevelDoor != null)
        {
            nextLevelDoor.GetComponent<Collider2D>().isTrigger = true;
        }

        // Save the updated puzzle state and door state
        SavePuzzleState(); // Call save after updating states
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

      
    }

    private void RandomizePlaintext()
    {
        // Reset the plaintextDescriptions to ensure it's fresh for each puzzle
        SetupPlaintextDescriptions();

        // Randomly select a plaintext and encrypt it to create a ciphertext
        currentPlaintextIndex = UnityEngine.Random.Range(0, possiblePlaintexts.Length);

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
            playerMovementScript.enabled = false; // Disable player movement
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
            DisplayFeedback(FeedbackMessage.Correct); // Show correct feedback
            puzzleSolved = true; // Mark the puzzle as solved
            SavePuzzleState(); // Save the puzzle state after solving
            timerRunning = false; // Stop the timer
            puzzleCanvas.SetActive(false); // Hide the puzzle canvas

            // Disable the collider after the puzzle is solved
            if (puzzleCollider != null)
            {
                puzzleCollider.enabled = false; // Disable the puzzle collider
            }
            notifRenderer = interactable.GetComponent<SpriteRenderer>();
            if (notifRenderer != null)
            {
                notifRenderer.enabled = false; // Hide the sprite initially
            }

            // Check if the word is already in the list to avoid duplicates
            if (!decipheredWords.Exists(dw => dw.word.Equals(plaintext, System.StringComparison.OrdinalIgnoreCase)))
            {
                decipheredWords.Add(new DecipheredWord(plaintext, plaintextDescriptions[plaintext])); // Store the solved word
                SaveDecipheredWords(); // Save deciphered words after solving the puzzle
            }

            // Show the correct answer panel with the plaintext
            ShowCorrectAnswerPanel(plaintext); // Ensure this is passing the correct plaintext
            EnableNextLevelDoorTrigger(); // Enable the next level door trigger

            SolvePuzzle();
        }
        else
        {
            DisplayFeedback(FeedbackMessage.Incorrect); // Show incorrect feedback
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

                // Enable the SpriteRenderer to show the sprite
                if (doorSpriteRenderer != null)
                {
                    doorSpriteRenderer.enabled = true; // Show the sprite
                    Debug.Log("nextLevelDoor sprite renderer enabled."); // Log success
                }
                else
                {
                    Debug.LogWarning("No SpriteRenderer found on nextLevelDoor."); // Log warning if no SpriteRenderer
                }
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
    public enum FeedbackMessage
    {
        Correct,
        Incorrect
    }

    private void DisplayFeedback(FeedbackMessage message)
    {
        switch (message)
        {
            case FeedbackMessage.Correct:
                feedbackText.text = "Correct!";
                break;
            case FeedbackMessage.Incorrect:
                feedbackText.text = "Try again!";
                break;
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

    public void AlbumButtonIG()
    {
        LoadDecipheredWords();  // Load the saved words
        DisplayAlbumWords();    // Display the words in the album
        correctAnswerPanel.SetActive(false);
        albumPanel.SetActive(true);
    }
    public void ProceedButtonIG() {
        albumPanel.SetActive(false);
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
    [SerializeField]
    private Transform wordListContainer;  // Container where deciphered words will be displayed

    [SerializeField]
    private GameObject wordItemPrefab;    // Prefab for displaying each word in the album

    [SerializeField]
    private TMP_Text descriptionText; // Text field to display the description

    public void ShowDescription(string description)
    {
        if (descriptionText != null)
        {
            descriptionText.text = description;  // Update the description text
        }
    }

    public void DisplayAlbumWords()
    {
        // Clear existing word items first
        foreach (Transform child in wordListContainer)
        {
            Destroy(child.gameObject);  // Destroy each existing child to avoid duplicates
        }

        // Instantiate a new word item for each deciphered word
        foreach (var word in decipheredWords)
        {
            GameObject wordItem = Instantiate(wordItemPrefab, wordListContainer);  // Create a new word item in the container

            TMP_Text wordText = wordItem.GetComponentInChildren<TMP_Text>();  // Get the TMP_Text component in the prefab
            if (wordText != null)
            {
                wordText.text = word.word;  // Set the word text
            }

            // Show description when the word is clicked
            Button wordButton = wordItem.GetComponent<Button>();
            if (wordButton != null)
            {
                string description = word.description;  // Copy the description for the button
                wordButton.onClick.AddListener(() => ShowDescription(description));  // Set button click action
            }
        }
    }


    // This method saves the deciphered words to PlayerPrefs
    public void SaveDecipheredWords()
    {
        // Load existing deciphered words from PlayerPrefs if they exist
        string existingJson = PlayerPrefs.GetString("DecipheredWords", string.Empty);
        List<DecipheredWord> existingWords = new List<DecipheredWord>();

        if (!string.IsNullOrEmpty(existingJson))
        {
            // Deserialize existing words
            WordsContainer existingContainer = JsonUtility.FromJson<WordsContainer>(existingJson);
            if (existingContainer != null)
            {
                existingWords = existingContainer.words;
            }
        }

        // Use a HashSet to merge existing words with new ones (to avoid duplicates)
        HashSet<DecipheredWord> mergedWords = new HashSet<DecipheredWord>(existingWords);
        mergedWords.UnionWith(decipheredWords);  // Add new deciphered words

        // Save the merged list of deciphered words back to PlayerPrefs
        string newJson = JsonUtility.ToJson(new WordsContainer { words = new List<DecipheredWord>(mergedWords) });
        PlayerPrefs.SetString("DecipheredWords", newJson);
        PlayerPrefs.Save();
    }



    // This method loads existing deciphered words from PlayerPrefs and updates the list
    public void LoadDecipheredWords()
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
