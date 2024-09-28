using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AlbumManager : MonoBehaviour
{
    [SerializeField] private GameObject wordItemPrefab; // Assign the prefab in the Inspector
    [SerializeField] private Transform wordListContainer; // Assign the content object of the Scroll View
    [SerializeField] private TMP_Text descriptionText; // Assign the description text UI element

    private List<DecipheredWord> decipheredWords = new List<DecipheredWord>();

    private void LoadDecipheredWords()
    {
        string json = PlayerPrefs.GetString("DecipheredWords", "{}");
        WordsContainer loadedWordsContainer = JsonUtility.FromJson<WordsContainer>(json);
        decipheredWords = loadedWordsContainer.words ?? new List<DecipheredWord>();

        Debug.Log("Loaded deciphered words: " + json);
        DisplayWords();
    }

    private void Start()
    {
        LoadDecipheredWords();
    }

    private void DisplayWords()
    {
        Debug.Log("Displaying words. Number of deciphered words: " + decipheredWords.Count);

        foreach (var word in decipheredWords)
        {
            // Instantiate a word item prefab for each word
            GameObject wordItem = Instantiate(wordItemPrefab, wordListContainer);

            TMP_Text wordText = wordItem.GetComponentInChildren<TMP_Text>();

            if (wordText != null)
            {
                wordText.text = word.word;
                Debug.Log("Displayed word: " + word.word);
            }
            else
            {
                Debug.LogError("WordItemPrefab does not contain a TMP_Text component.");
            }

            // Add a listener for clicking the button
            Button wordButton = wordItem.GetComponent<Button>();
            wordButton.onClick.AddListener(() => ShowDescription(word.description));
            
            // Check if the word has been deciphered to disable the collider
            if (word.isDeciphered)
            {
                // Assuming there's a collider to disable, handle it accordingly
                wordItem.GetComponent<Collider2D>().enabled = false; // Disable the collider
                wordButton.interactable = false; // Disable the button to prevent further interaction
            }
        }
    }

    private void ShowDescription(string description)
    {
        descriptionText.text = description;
    }

    // Call this method when the player successfully answers the puzzle
    public void OnPuzzleSolved(DecipheredWord solvedWord)
    {
        // Mark the word as deciphered
        solvedWord.isDeciphered = true;

        // Save the updated list of words to PlayerPrefs
        SaveDecipheredWords();

        // Disable the collider (replace with your actual logic for disabling the relevant collider)
        // Example: If you have a collider associated with the word, disable it here
        // colliderToDisable.enabled = false;
    }

    private void SaveDecipheredWords()
    {
        WordsContainer container = new WordsContainer { words = decipheredWords };
        string json = JsonUtility.ToJson(container);
        PlayerPrefs.SetString("DecipheredWords", json);
        PlayerPrefs.Save(); // Save the PlayerPrefs changes
        Debug.Log("Saved deciphered words: " + json);
    }

    [System.Serializable]
    private class WordsContainer
    {
        public List<DecipheredWord> words;
    }
}

// Ensure this class has the necessary fields to be serialized
[System.Serializable]
public class DecipheredWord
{
    public string word;
    public string description;
    public bool isDeciphered; // New field to track whether the word has been deciphered
}
