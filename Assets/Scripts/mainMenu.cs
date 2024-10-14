using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    public void startBTN() {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene(2);
    }
    public void settingsBTN() {
        SceneManager.LoadScene(13);
    }
    public void albumBTN() {
        SceneManager.LoadScene(12);
    }
    public void tutorialBTN() { 
        
    }
    public void closeAlbum() {
        SceneManager.LoadScene(1);
    }
    public void backBTN() {
        SceneManager.LoadScene(1);
    }
}
