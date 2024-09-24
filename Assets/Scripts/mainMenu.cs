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
        
    }
    public void albumBTN() {
        SceneManager.LoadScene(8);
    }
    public void tutorialBTN() { 
        
    }
    public void closeAlbum() {
        SceneManager.LoadScene(1);
    }
}
