using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PureDialogue : MonoBehaviour
{
    // UI References for Dialogue
    [SerializeField]
    private GameObject dialogueCanvas2;

    [SerializeField]
    private TMP_Text speakerText2;

    [SerializeField]
    private TMP_Text dialogueText2;

    [SerializeField]
    private Image portraitImage2;

    // Dialogue Content
    [SerializeField]
    private string[] speaker2;

    [SerializeField]
    [TextArea]
    private string[] dialogueWords2;

    [SerializeField]
    private Sprite[] portrait2;

    private bool dialogueActivated2;
    private int step;

    // Reference to PlayerMovement and Animator
    [SerializeField]
    private NewBehaviourScript playerMovement; // Reference to your PlayerMovement script

    private Animator playerAnimator; // Reference to the player's Animator

    private void Start()
    {
        // Set up initial UI states
        dialogueCanvas2.SetActive(false);

        // Get the Animator component from the PlayerMovement script
        if (playerMovement != null)
        {
            playerAnimator = playerMovement.GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Talk") && dialogueActivated2)
        {
            if (step >= speaker2.Length) // Check if dialogue is over
            {
                dialogueCanvas2.SetActive(false);
                step = 0; // Optionally reset dialogue to start from the beginning
                dialogueActivated2 = false; // Optionally disable dialogue activation

                // Re-enable player movement and animation
                if (playerMovement != null)
                {
                    playerMovement.enabled = true;
                }
                if (playerAnimator != null)
                {
                    playerAnimator.SetBool("run", true); // Re-enable running animation
                }
            }
            else
            {
                dialogueCanvas2.SetActive(true);
                speakerText2.text = speaker2[step];
                dialogueText2.text = dialogueWords2[step];
                portraitImage2.sprite = portrait2[step];
                step++;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dialogueActivated2 = true;

            // Disable player movement and animation when dialogue starts
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("run", false); // Disable running animation
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Hide the dialogue canvas if the player exits the trigger zone
            if (dialogueCanvas2.activeInHierarchy)
            {
                dialogueCanvas2.SetActive(false);
            }
            dialogueActivated2 = false;
        }
    }
}
