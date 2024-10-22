using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class tutorialButton : MonoBehaviour
{
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private GameObject tutorialButtons;
    [SerializeField] private GameObject caesarTutorial;
    [SerializeField] private GameObject keywordTutorial;
    [SerializeField] private GameObject giovanniTutorial;
    [SerializeField] private GameObject transpoTutorial;
    [SerializeField] private NewBehaviourScript playerMovementScript;

    public void openTutorialMenu() { 
        tutorialCanvas.SetActive(true);
        tutorialButtons.SetActive(true);
        playerMovementScript.enabled = false;
        playerMovementScript.body.velocity = Vector2.zero;
    }
    public void closeTutorialMenu() {
        tutorialCanvas.SetActive(false);
        caesarTutorial.SetActive(false);
        keywordTutorial.SetActive(false);
        giovanniTutorial.SetActive(false);
        transpoTutorial.SetActive(false);
        tutorialButtons.SetActive(false);
        playerMovementScript.enabled = true;
    }
    public void openCaesarTutorial() { 
        caesarTutorial.SetActive(true);
        keywordTutorial.SetActive(false);
        giovanniTutorial.SetActive(false);
        transpoTutorial.SetActive(false);
        tutorialCanvas.SetActive(false);
        playerMovementScript.enabled = false;
        playerMovementScript.body.velocity = Vector2.zero;
    }
    public void openKeywordTutorial()
    {
        caesarTutorial.SetActive(false);
        keywordTutorial.SetActive(true);
        giovanniTutorial.SetActive(false);
        transpoTutorial.SetActive(false);
        tutorialCanvas.SetActive(false);
        playerMovementScript.enabled = false;
        playerMovementScript.body.velocity = Vector2.zero;
    }
    public void openGiovanniTutorial()
    {
        caesarTutorial.SetActive(false);
        keywordTutorial.SetActive(false);
        giovanniTutorial.SetActive(true);
        transpoTutorial.SetActive(false);
        tutorialCanvas.SetActive(false);
        playerMovementScript.enabled = false;
        playerMovementScript.body.velocity = Vector2.zero;
    }
    public void openTranspoTutorial()
    {
        caesarTutorial.SetActive(false);
        keywordTutorial.SetActive(false);
        giovanniTutorial.SetActive(false);
        transpoTutorial.SetActive(true);
        tutorialCanvas.SetActive(false);
        playerMovementScript.enabled = false;
        playerMovementScript.body.velocity = Vector2.zero;
    }

}
