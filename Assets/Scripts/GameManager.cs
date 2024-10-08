using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // List to hold completed puzzle indices
    private List<int> completedPuzzleIndices = new List<int>();

    // Reference to the player object
    [SerializeField] private GameObject player;

    // Call this method to save the game progress
    public void SaveProgress()
    {
        // Save the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("CurrentScene", currentSceneIndex);

        // Save the puzzles the player has completed
        string completedPuzzles = string.Join(",", completedPuzzleIndices); // Convert list to string
        PlayerPrefs.SetString("CompletedPuzzles", completedPuzzles);

        PlayerPrefs.Save(); // Ensure the PlayerPrefs are saved
        Debug.Log("Game progress saved.");
    }

    public void LoadProgress()
    {
        // Check if a previous game exists
        if (PlayerPrefs.HasKey("CurrentScene"))
        {
            // Load the saved scene
            int savedSceneIndex = PlayerPrefs.GetInt("CurrentScene");
            SceneManager.LoadScene(savedSceneIndex);

            // Load the completed puzzles
            string savedPuzzles = PlayerPrefs.GetString("CompletedPuzzles", "");
            if (!string.IsNullOrEmpty(savedPuzzles))
            {
                string[] puzzleIndices = savedPuzzles.Split(',');
                foreach (string index in puzzleIndices)
                {
                    if (int.TryParse(index, out int puzzleIndex))
                    {
                        completedPuzzleIndices.Add(puzzleIndex); // Rebuild the list
                    }
                }
            }

            Debug.Log("Game progress loaded.");
        }
        else
        {
            Debug.Log("No saved progress found.");
        }
    }

    public void NewGame()
    {
        // Clear any existing saved progress
        PlayerPrefs.DeleteAll();

        // Load the first scene (usually index 0)
        SceneManager.LoadScene(4);

        // Reset any other data such as puzzle completion
        completedPuzzleIndices.Clear();
        Debug.Log("New game started.");
    }

    private void Start()
    {
        // Set the player's position at the spawn point when the scene starts
        SetPlayerSpawnPosition();
    }

    private void SetPlayerSpawnPosition()
    {
        // Find the spawn point in the current scene
        GameObject spawnPoint = GameObject.Find("PlayerSpawn");
        if (spawnPoint != null && player != null)
        {
            player.transform.position = spawnPoint.transform.position; // Set the player's position
            Debug.Log("Player positioned at spawn point: " + spawnPoint.transform.position);
        }
    }
}
