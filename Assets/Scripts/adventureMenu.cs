using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class adventureMenu : MonoBehaviour
{
    public void continueBTN() {
        SceneManager.LoadScene(6);
    }
    public void backBTN() {
        SceneManager.LoadScene(2);
    }
}
