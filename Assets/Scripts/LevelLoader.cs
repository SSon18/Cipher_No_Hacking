using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;
    [SerializeField] Animator transitionAnim;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to go to the next level
    public void NextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    // Method to go to the previous level
    public void PreviousLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex - 1));
    }

    // Coroutine to load a scene with transition animation
    IEnumerator LoadLevel(int sceneIndex)
    {
        // Start the transition animation
        transitionAnim.SetTrigger("End");

        // Wait for the animation or a delay if needed
        yield return new WaitForSeconds(0.2f);  // Adjust time based on your transition animation length

        // Load the requested scene
        SceneManager.LoadSceneAsync(sceneIndex);

        // Trigger animation for entering the scene
        transitionAnim.SetTrigger("Start");
    }
}
