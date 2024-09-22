using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static PuzzleSystem;

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

    // Ensure this method is called in Start
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
        }
    }


    private void ShowDescription(string description)
    {
        descriptionText.text = description;
    }

    [System.Serializable]
    private class WordsContainer
    {
        public List<DecipheredWord> words;
    }
}
