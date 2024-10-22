using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class missionButtonScript : MonoBehaviour
{
    [SerializeField] private NewBehaviourScript playerMovementScript;
    [SerializeField] private GameObject missionCanva;

    public void showMission() { 
        missionCanva.SetActive(true);
        playerMovementScript.enabled = false; // Disable player movement
        playerMovementScript.body.velocity = Vector2.zero;
    }
    public void closeMission() {
        missionCanva.SetActive(false);
        playerMovementScript.enabled = true; // Disable player movement
    }
}
