using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using static PuzzleSystem;
using UnityEngine.SceneManagement;
using System.Linq.Expressions;
using SupanthaPaul;

public class Giovanni : MonoBehaviour
{
    // UI Components
    [SerializeField] public GameObject cipherPuzzleCanvas;
    [SerializeField] public GameObject bookAlbumCanvas;
    [SerializeField] private TMP_Text ciphertextText;
    [SerializeField] public TMP_InputField answerInputField;
    [SerializeField] public TMP_Text feedbackText;
    [SerializeField] private Button submitButton;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text missionText;
    [SerializeField] private GameObject correctAnswerPanel;
    [SerializeField] private TMP_Text correctAnswerWordText;
    [SerializeField] private TMP_Text correctAnswerDescriptionText;
    [SerializeField] private Button proceedButton;

    // Cipher Configuration
    [SerializeField] private string keyword; // The keyword for the cipher
    [SerializeField] private char keyLetter; // The starting letter for Giovanni's cipher
    [SerializeField] private string[] possiblePlaintexts;
    [SerializeField] private string[] descriptions;
    private Dictionary<string, string> plaintextDescriptions;
    private string currentPlaintext;
    private string currentCiphertext;
    private bool puzzleSolved = false;
    private int currentPlaintextIndex;
    [SerializeField][TextArea] private string[] missionWords;

    // Timer Settings
    [SerializeField] private float timeLimit = 30.0f;
    private float timer;
    private bool timerRunning = false;

    // References
    [SerializeField] private NewBehaviourScript playerMovementScript;
    [SerializeField] private CameraControl cameraControlScript;
    [SerializeField] public giovanniDialogue dialogueSystem;
    [SerializeField] private GameObject nextLevelDoor;
    private SpriteRenderer doorSpriteRenderer;
    private BoxCollider2D puzzleCollider;

    [SerializeField]
    private Transform wordListContainer;  // Container where deciphered words will be displayed

    [SerializeField]
    private GameObject wordItemPrefab;    // Prefab for displaying each word in the album

    [SerializeField]
    private TMP_Text descriptionText; // Text field to display the description

    private List<DecipheredWord> decipheredWords = new List<DecipheredWord>(); // List of deciphered words
    [SerializeField]
    public GameObject albumPanel;
    private bool puzzleActivated; // Indicates if the puzzle is activated

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
        cipherPuzzleCanvas.SetActive(false); // Hide the puzzle canvas at the start
        submitButton.onClick.AddListener(OnSubmit); // Add listener for submit button
        proceedButton.onClick.AddListener(OnProceed); // Add listener for Proceed button

        // Ensure DialogueSystem is assigned correctly
        if (dialogueSystem == null)
        {
            dialogueSystem = FindObjectOfType<giovanniDialogue>();
            if (dialogueSystem == null)
            {
                Debug.LogError("DialogueSystem not found in the scene!");
            }

        }
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
               
            }
            UpdateTimerDisplay();
        }
    }
    private void UpdateTimerDisplay()
    {
        timerText.text = "Time Left: " + Mathf.Ceil(timer).ToString();
    }
   

    public void OnProceed()
    {
        correctAnswerPanel.SetActive(false);
        playerMovementScript.enabled = true;
        playerMovementScript.speed = 5;
        cameraControlScript.enabled = true;
        // Reset puzzle state for reactivation
        puzzleSolved = false;
        dialogueSystem.ResetDialogue();

        // Optionally, reset the puzzle UI if you want to show it again
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
        if (cameraControlScript != null)
        {
            cameraControlScript.enabled = true;
        }
        if (playerMovementScript.speed == 0)
        {
            playerMovementScript.speed = 5;
        }
        // Reset puzzle activation state
        puzzleActivated = false; // Set this back to false to indicate the puzzle is no longer active

        // Optionally, reset dialogue for the next puzzle
        if (dialogueSystem != null)
        {
            dialogueSystem.ResetDialogue(); // Reset the dialogue state
        }
    }

    public void ActivatePuzzle()
    {
        if (!puzzleSolved)
        {
            cipherPuzzleCanvas.SetActive(true);
            bookAlbumCanvas.SetActive(false);
            timer = timeLimit;
            timerRunning = true;
            answerInputField.interactable = true;
            answerInputField.text = "";
            answerInputField.Select();
            answerInputField.ActivateInputField();
            playerMovementScript.enabled = false;
            playerMovementScript.speed = 0;
            cameraControlScript.enabled = false;

            DisplayMissionText();
        }
    }

    private void DisplayMissionText()
    {
        // Display the mission text based on the mission words
        if (missionText != null && missionWords.Length > 0)
        {
            missionText.text = "Mission: " + missionWords[0];
        }
    }

    private void SetupPlaintextDescriptions()
    {
        plaintextDescriptions = new Dictionary<string, string>();

        for (int i = 0; i < possiblePlaintexts.Length; i++)
        {
            if (!string.IsNullOrEmpty(possiblePlaintexts[i]) && !string.IsNullOrEmpty(descriptions[i]))
            {
                plaintextDescriptions.Add(possiblePlaintexts[i], descriptions[i]);
            }
        }
    }

    private void RandomizePlaintext()
    {
        if (possiblePlaintexts.Length == 0) return;

        currentPlaintextIndex = UnityEngine.Random.Range(0, possiblePlaintexts.Length);
        currentPlaintext = possiblePlaintexts[currentPlaintextIndex];

        if (!string.IsNullOrEmpty(currentPlaintext))
        {
            currentCiphertext = EncryptWithGiovannisCipher(currentPlaintext, keyword, keyLetter);
            ciphertextText.text = "Ciphertext: " + currentCiphertext;
        }
    }

    private string EncryptWithGiovannisCipher(string plaintext, string keyword, char keyLetter)
    {
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string shiftedAlphabet = ShiftAlphabetFromKeyLetter(alphabet, keyLetter);
        string keyedAlphabet = GenerateKeyedAlphabet(keyword, shiftedAlphabet);

        string ciphertext = "";
        foreach (char c in plaintext.ToUpper())
        {
            if (alphabet.Contains(c.ToString()))
            {
                int index = alphabet.IndexOf(c); // Find index in the normal alphabet
                ciphertext += keyedAlphabet[index]; // Map to the keyed alphabet
            }
            else
            {
                ciphertext += c; // Preserve non-alphabet characters
            }
        }
        return ciphertext;
    }

    private string ShiftAlphabetFromKeyLetter(string alphabet, char keyLetter)
    {
        int keyIndex = alphabet.IndexOf(char.ToUpper(keyLetter));
        if (keyIndex == -1) return alphabet; // Invalid keyLetter

        // Shift the alphabet starting from the key letter
        string shiftedAlphabet = alphabet.Substring(keyIndex) + alphabet.Substring(0, keyIndex);
        return shiftedAlphabet;
    }

    private string GenerateKeyedAlphabet(string keyword, string shiftedAlphabet)
    {
        HashSet<char> usedLetters = new HashSet<char>();
        string keyedAlphabet = "";

        // Add unique letters from the keyword to the keyed alphabet
        foreach (char c in keyword.ToUpper())
        {
            if (!usedLetters.Contains(c) && shiftedAlphabet.Contains(c.ToString()))
            {
                keyedAlphabet += c;
                usedLetters.Add(c);
            }
        }

        // Add the rest of the letters from the shifted alphabet
        foreach (char c in shiftedAlphabet)
        {
            if (!usedLetters.Contains(c))
            {
                keyedAlphabet += c;
            }
        }

        return keyedAlphabet;
    }
    private SpriteRenderer notifRenderer;
    private GameObject notif;
    [SerializeField]
    private GameObject interactable;
    public void OnSubmit()
    {
        // Handle answer submission from the input field
        string userAnswer = answerInputField.text.Trim(); // Trim any whitespace

        // Check if the user's answer matches the plaintext
        if (userAnswer.Equals(currentPlaintext, System.StringComparison.OrdinalIgnoreCase))
        {
            DisplayFeedback(FeedbackMessage.Correct); // Show correct feedback
            puzzleSolved = true; // Mark the puzzle as solved
            SavePuzzleState(); // Save the puzzle state after solving
            timerRunning = false; // Stop the timer
            cipherPuzzleCanvas.SetActive(false); // Hide the puzzle canvas
            bookAlbumCanvas.SetActive(true);

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
            if (!decipheredWords.Exists(dw => dw.word.Equals(currentPlaintext, System.StringComparison.OrdinalIgnoreCase)))
            {
                decipheredWords.Add(new DecipheredWord(currentPlaintext, plaintextDescriptions[currentPlaintext])); // Store the solved word
                SaveDecipheredWords(); // Save deciphered words after solving the puzzle
            }

            // Show the correct answer panel with the plaintext
            ShowCorrectAnswerPanel(currentPlaintext); // Ensure this is passing the correct plaintext
            EnableNextLevelDoorTrigger(); // Enable the next level door trigger

            SolvePuzzle();
        }
        else
        {
            DisplayFeedback(FeedbackMessage.Incorrect); // Show incorrect feedback
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


    public void LoadDecipheredWords()
    {
        // Clear existing deciphered words to avoid duplicates
        decipheredWords.Clear();

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
                    if (!decipheredWords.Exists(dw => dw.word.Equals(word.word, StringComparison.OrdinalIgnoreCase)))
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



    private void ShowCorrectAnswerPanel(string word)
    {
        correctAnswerWordText.text = "Deciphered Word: " + word;
        correctAnswerDescriptionText.text = plaintextDescriptions[word];
        correctAnswerPanel.SetActive(true);

        // Automatically add the deciphered word and description to the album
        decipheredWords.Add(new DecipheredWord(word, plaintextDescriptions[word])); // Add to list
        SaveDecipheredWords(); // Ensure it's saved
    }



    private void EnableNextLevelDoorTrigger()
    {
        if (nextLevelDoor != null)
        {
            BoxCollider2D doorCollider = nextLevelDoor.GetComponent<BoxCollider2D>();
            if (doorCollider != null)
            {
                doorCollider.isTrigger = true;
                if (doorSpriteRenderer != null)
                {
                    doorSpriteRenderer.enabled = true;
                }
            }
        }
    }
}
