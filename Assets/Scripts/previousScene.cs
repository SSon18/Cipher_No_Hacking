using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class previousScene : MonoBehaviour
{
    [SerializeField] Animator transitionAnim;
    IEnumerator LoadLevel(int sceneIndex)
    {
      
       // Wait for the animation or a delay if needed
        yield return new WaitForSeconds(0.2f);  // Adjust time based on your transition animation length

        // Load the requested scene
        SceneManager.LoadSceneAsync(sceneIndex);

    }
    public void PreviousLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex - 1));
    }
}
