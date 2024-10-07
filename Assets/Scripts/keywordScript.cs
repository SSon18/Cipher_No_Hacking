using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static PuzzleSystem;
using System;

public class KeywordCipherSystem : MonoBehaviour
{
    // UI Components
    [SerializeField] public GameObject keywordPuzzleCanvas; // The canvas for the keyword puzzle
    [SerializeField] private TMP_Text ciphertextText; // Text display for the keyword ciphertext
    [SerializeField] public TMP_InputField answerInputField; // Input field for user's answer
    [SerializeField] public TMP_Text feedbackText; // Feedback display text
    [SerializeField] private Button submitButton; // Submit button for answers
    [SerializeField] private TMP_Text timerText; // Text display for timer
    [SerializeField] private TMP_Text missionText; // Text display for mission instructions
    [SerializeField] private GameObject correctAnswerPanel; // Panel to show when the player gets the correct answer
    [SerializeField] private TMP_Text correctAnswerWordText; // Text to display the correct answer
    [SerializeField] private TMP_Text correctAnswerDescriptionText; // Text to display the correct answer's description
    [SerializeField] private Button proceedButton; // Proceed button for next steps
    

    // Cipher Configuration
    [SerializeField] private string keyword = "KEYWORD"; // The keyword for the cipher
    [SerializeField] private string[] possiblePlaintexts; // Array of potential plaintexts
    [SerializeField] private string[] descriptions; // Descriptions for the plaintexts
    private Dictionary<string, string> plaintextDescriptions; // Dictionary to pair plaintexts with descriptions
    private string currentPlaintext; // Current plaintext to solve
    private string currentCiphertext; // Current ciphertext shown to the player
    private bool puzzleSolved = false; // Track if puzzle has been solved
    private int currentPlaintextIndex; // Index for current plaintext
    [SerializeField]
    [TextArea]
    private string[] missionWords;

    // Timer Settings
    [SerializeField] private float timeLimit = 30.0f; // Time limit for solving the puzzle
    private float timer; // Timer variable
    private bool timerRunning = false; // Track if the timer is running

    // References
    [SerializeField] private NewBehaviourScript playerMovementScript; // Reference to player movement script
    [SerializeField] public keywordDialogue dialogueSystem; // Reference to the dialogue system
    [SerializeField] private GameObject nextLevelDoor; // Object for the door leading to the next level
    private SpriteRenderer doorSpriteRenderer; // Declare a SpriteRenderer reference
    private PuzzleSystem puzzleSystem;
    private BoxCollider2D puzzleCollider; // Assuming you are using a BoxCollider2D
    private List<DecipheredWord> decipheredWords = new List<DecipheredWord>();
    private string plaintext; // The current plaintext to solve
    [SerializeField]
    private Transform wordListContainer;  // Container where deciphered words will be displayed
    private bool puzzleActivated; // Indicates if the puzzle is activated
    [SerializeField]
    private GameObject wordItemPrefab;    // Prefab for displaying each word in the album
    [SerializeField]
    private GameObject interactable;


    [SerializeField]
    private TMP_Text descriptionText; // Text field to display the description
    [SerializeField] public GameObject albumPanel;
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

    private void Start()
    {
        LoadPuzzleState();
        LoadDecipheredWords();
        doorSpriteRenderer = nextLevelDoor.GetComponent<SpriteRenderer>();
        if (doorSpriteRenderer != null)
        {
            doorSpriteRenderer.enabled = false; // Hide the sprite initially
        }
        SetupPlaintextDescriptions();
        RandomizePlaintext();

        keywordPuzzleCanvas.SetActive(false);
        submitButton.onClick.AddListener(OnSubmit);
        proceedButton.onClick.AddListener(OnProceed);

        if (dialogueSystem == null)
        {
            dialogueSystem = FindObjectOfType<keywordDialogue>();
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
    public void AlbumButtonIG()
    {
        LoadDecipheredWords();  // Load the saved words
        DisplayAlbumWords();    // Display the words in the album

        correctAnswerPanel.SetActive(false);
        albumPanel.SetActive(true);

        // Refresh the album to ensure the latest deciphered word is displayed
        DisplayAlbumWords();
    }

    public void ProceedButtonIG()
    {
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
    public void ShowDescription(string description)
    {
        if (descriptionText != null)
        {
            descriptionText.text = description;  // Update the description text
        }
    }
    // Helper class to serialize the list of words
    [System.Serializable]
    private class WordsContainer
    {
        public List<DecipheredWord> words; // List of deciphered words
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
                feedbackText.text = "Time's up! Try again.";
                ResetPuzzle();
            }
            UpdateTimerDisplay();
        }
    }

    private void SetupPlaintextDescriptions()
    {
        plaintextDescriptions = new Dictionary<string, string>();

        if (possiblePlaintexts.Length != descriptions.Length)
        {
            Debug.LogError("PossiblePlaintexts and Descriptions arrays are not the same length!");
        }

        for (int i = 0; i < possiblePlaintexts.Length; i++)
        {
            if (!string.IsNullOrEmpty(possiblePlaintexts[i]) && !string.IsNullOrEmpty(descriptions[i]))
            {
                plaintextDescriptions.Add(possiblePlaintexts[i], descriptions[i]);
                Debug.Log("Added to dictionary: " + possiblePlaintexts[i] + " -> " + descriptions[i]);
            }
            else
            {
                Debug.LogWarning("Plaintext or description is missing at index " + i);
            }
        }
    }



    private void RandomizePlaintext()
    {
        if (possiblePlaintexts.Length == 0)
        {
            Debug.LogError("No possible plaintexts are set! Make sure the array is populated in the Inspector.");
            return;
        }

        currentPlaintextIndex = UnityEngine.Random.Range(0, possiblePlaintexts.Length);
        currentPlaintext = possiblePlaintexts[currentPlaintextIndex];

        if (!string.IsNullOrEmpty(currentPlaintext))
        {
            currentCiphertext = EncryptWithKeyword(currentPlaintext, keyword);
            ciphertextText.text = "Ciphertext: " + currentCiphertext;
            Debug.Log("Selected Plaintext: " + currentPlaintext);
            Debug.Log("Corresponding Ciphertext: " + currentCiphertext);
        }
        else
        {
            Debug.LogError("The selected plaintext is null or empty at index " + currentPlaintextIndex);
        }
    }



    public void ActivatePuzzle()
    {
        if (!puzzleSolved)
        {
            keywordPuzzleCanvas.SetActive(true);
            timer = timeLimit;
            timerRunning = true;
            answerInputField.interactable = true;
            answerInputField.text = "";
            answerInputField.Select();
            answerInputField.ActivateInputField();
            playerMovementScript.enabled = false;
            DisplayMissionText();
        }
    }

    private string EncryptWithKeyword(string plaintext, string keyword)
    {
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string keyedAlphabet = GenerateKeyedAlphabet(keyword, alphabet);
        string ciphertext = "";

        foreach (char c in plaintext.ToUpper())
        {
            if (alphabet.Contains(c.ToString()))
            {
                int index = alphabet.IndexOf(c);
                ciphertext += keyedAlphabet[index];
            }
            else
            {
                ciphertext += c;
            }
        }

        return ciphertext;
    }

    private string GenerateKeyedAlphabet(string keyword, string alphabet)
    {
        HashSet<char> usedLetters = new HashSet<char>();
        string keyedAlphabet = "";

        foreach (char c in keyword.ToUpper())
        {
            if (!usedLetters.Contains(c) && alphabet.Contains(c.ToString()))
            {
                keyedAlphabet += c;
                usedLetters.Add(c);
            }
        }

        foreach (char c in alphabet)
        {
            if (!usedLetters.Contains(c))
            {
                keyedAlphabet += c;
            }
        }

        return keyedAlphabet;
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
        string userAnswer = answerInputField.text.Trim().ToUpper();

        // Log the current plaintext for debugging
        Debug.Log("User's answer: " + userAnswer);
        Debug.Log("Current plaintext: " + currentPlaintext);

        // Check if currentPlaintext is valid
        if (string.IsNullOrEmpty(currentPlaintext))
        {
            Debug.LogError("Current plaintext is null or empty! Cannot validate answer.");
            return;
        }

        // Check if the plaintext exists in the dictionary
        if (!plaintextDescriptions.ContainsKey(currentPlaintext))
        {
            Debug.LogWarning("Plaintext is null, empty, or missing from the descriptions.");
            return;
        }

        // Continue with normal puzzle validation
        if (userAnswer == currentPlaintext.ToUpper())
        {
            puzzleSolved = true;
            SavePuzzleState();
            timerRunning = false;
            feedbackText.text = "Correct!";
            ShowCorrectAnswerPanel(currentPlaintext);
            keywordPuzzleCanvas.SetActive(false);
            EnableNextLevelDoorTrigger();

            // Handle deciphered words
            if (!decipheredWords.Exists(dw => dw.word.Equals(currentPlaintext, System.StringComparison.OrdinalIgnoreCase)))
            {
                decipheredWords.Add(new DecipheredWord(currentPlaintext, plaintextDescriptions[currentPlaintext]));
                SaveDecipheredWords();
            }
            SolvePuzzle();
        }
        else
        {
            feedbackText.text = "Incorrect, try again!";
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
                
            }
        }
    }
   

    private void ShowCorrectAnswerPanel(string word)
    {
        correctAnswerWordText.text = "Deciphered Word: " + word;
        correctAnswerDescriptionText.text = plaintextDescriptions[word];
        correctAnswerPanel.SetActive(true);
    }

    private void ResetPuzzle()
    {
        RandomizePlaintext();
        answerInputField.text = "";
        timer = timeLimit;
        timerRunning = true;
    }

    private void UpdateTimerDisplay()
    {
        timerText.text = "Time Left: " + Mathf.Ceil(timer).ToString();
    }

    public void OnProceed()
    {
        correctAnswerPanel.SetActive(false);
        playerMovementScript.enabled = true;
        puzzleSolved = false;
        dialogueSystem.ResetDialogue();
    }
}
