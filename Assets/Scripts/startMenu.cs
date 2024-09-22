using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startMenu : MonoBehaviour
{
    public void adventureBTN() {
        SceneManager.LoadScene(3);
    }
    public void minigameBTN() { 
        
    }
    public void backBTN() {
        SceneManager.LoadScene(1);
    }
    public void viewAlbumBTN() {
        SceneManager.LoadScene(8);
    }
    public void doneBTN() {
        SceneManager.LoadScene(5);
    }
    public void OnViewAlbumButtonClick()
    {
        // Use SceneManager to load the Album scene
        SceneManager.LoadScene(8);
    }
}
