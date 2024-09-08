using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    // UI References for Dialogue
    [SerializeField]
    private GameObject dialogueCanvas;

    [SerializeField]
    private TMP_Text speakerText;

    [SerializeField]
    private TMP_Text dialogueText;

    [SerializeField]
    private Image portraitImage;

    // Dialogue Content
    [SerializeField]
    private string[] speaker;

    [SerializeField]
    [TextArea]
    private string[] dialogueWords;

    [SerializeField]
    private Sprite[] portrait;

    private bool dialogueActivated;
    private int step;

    // Reference to the Puzzle System
    [SerializeField]
    private PuzzleSystem puzzleSystem;

    private void Start()
    {
        // Set up initial UI states
        dialogueCanvas.SetActive(false);

        // Ensure PuzzleSystem is assigned correctly
        if (puzzleSystem == null)
        {
            puzzleSystem = FindObjectOfType<PuzzleSystem>();

            if (puzzleSystem == null)
            {
                Debug.LogError("PuzzleSystem not found in the scene!");
            }
        }
    }

    private void Update()
    {
        // Prevent dialogue interaction if the game is paused (Time.timeScale == 0)
        if (Time.timeScale == 0) return;

        if (Input.GetButtonDown("Talk") && dialogueActivated)
        {
            if (step >= speaker.Length) // Check if dialogue is over
            {
                dialogueCanvas.SetActive(false);
                //step = 0; // repeat dialogue

                // Activate puzzle system once dialogue ends
                puzzleSystem.ActivatePuzzle();
            }
            else
            {
                dialogueCanvas.SetActive(true);
                speakerText.text = speaker[step];
                dialogueText.text = dialogueWords[step];
                portraitImage.sprite = portrait[step];
                step++;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            dialogueActivated = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Check if the dialogueCanvas is still available and active
            if (dialogueCanvas != null && dialogueCanvas.activeInHierarchy)
            {
                dialogueActivated = false;
                dialogueCanvas.SetActive(false);
            }
        }
    }
}
