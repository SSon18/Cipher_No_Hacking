using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject PausePanel;

    void Start()
    {
        // Ensure PausePanel is inactive at the start
        PausePanel.SetActive(false);
    }

    public void Pause()
    {
        Debug.Log("Pausing game and showing pause menu");
        PausePanel.SetActive(true);
        Time.timeScale = 0; // Pauses the game
        ToggleNPCColliders(false); // Disable NPC colliders
    }

    public void Continue()
    {
        Debug.Log("Resuming game and hiding pause menu");
        PausePanel.SetActive(false);
        Time.timeScale = 1; // Resumes the game
        ToggleNPCColliders(true); // Enable NPC colliders
    }

    public void Exit()
    {
        SceneManager.LoadScene(2);
        Time.timeScale = 1;
    }

    private void ToggleNPCColliders(bool enable)
    {
        // Find all GameObjects with the tag "NPC"
        GameObject[] npcObjects = GameObject.FindGameObjectsWithTag("NPC");

        // Loop through each NPC and enable/disable the BoxCollider2D
        foreach (GameObject npc in npcObjects)
        {
            BoxCollider2D npcCollider = npc.GetComponent<BoxCollider2D>();
            if (npcCollider != null)
            {
                npcCollider.enabled = enable;
            }
        }
    }
}
