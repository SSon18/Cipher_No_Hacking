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
}
