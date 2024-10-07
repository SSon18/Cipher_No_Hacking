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

        // Save the player's position
        Vector3 playerPosition = player.transform.position;
        PlayerPrefs.SetFloat("PlayerPosX", playerPosition.x);
        PlayerPrefs.SetFloat("PlayerPosY", playerPosition.y);
        PlayerPrefs.SetFloat("PlayerPosZ", playerPosition.z);

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
                    int puzzleIndex;
                    if (int.TryParse(index, out puzzleIndex))
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

    // This method is called after the scene is loaded
    private void OnLevelWasLoaded(int level)
    {
        // If player position data exists, load the player's position
        if (PlayerPrefs.HasKey("PlayerPosX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");
            float z = PlayerPrefs.GetFloat("PlayerPosZ");

            player.transform.position = new Vector3(x, y, z); // Set the player's position
            Debug.Log("Player position loaded.");
        }
    }
}
