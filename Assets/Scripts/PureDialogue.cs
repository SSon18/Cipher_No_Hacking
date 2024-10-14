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
    private bool isDisplaying; // New flag to check if displaying dialogue

    // Reference to PlayerMovement and Animator
    [SerializeField]
    private NewBehaviourScript playerMovement; // Reference to your PlayerMovement script

    private Animator playerAnimator; // Reference to the player's Animator
    private SpriteRenderer notifRenderer;
    private GameObject notif;
    [SerializeField]
    private GameObject interactable;
    [SerializeField]
    private GameObject nextLevelDoor; // Object for the door leading to the next level
    private SpriteRenderer doorSpriteRenderer; // Declare a SpriteRenderer reference
    private void Start()
    {
        doorSpriteRenderer = nextLevelDoor.GetComponent<SpriteRenderer>();
        if (doorSpriteRenderer != null)
        {
            doorSpriteRenderer.enabled = false; // Hide the sprite initially
        }
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
        if (Input.GetButtonDown("Talk") && dialogueActivated2 && !isDisplaying) // Check if not displaying
        {
            if (step >= speaker2.Length) // Check if dialogue is over
            {            
                dialogueCanvas2.SetActive(false);
                step = 0; // Optionally reset dialogue to start from the beginning
                dialogueActivated2 = false; // Optionally disable dialogue activation
                notifRenderer = interactable.GetComponent<SpriteRenderer>();
                if (notifRenderer != null)
                {
                    notifRenderer.enabled = false; // Hide the sprite initially
                }
                // Re-enable player movement and animation
                if (playerMovement != null)
                {
                    playerMovement.enabled = true;
                }
                if (playerAnimator != null)
                {
                    playerAnimator.SetBool("run", true); // Re-enable running animation
                }
                EnableNextLevelDoorTrigger();
            }
            else
            {
                dialogueCanvas2.SetActive(true);
                speakerText2.text = speaker2[step];
                StartCoroutine(DisplayDialogue(dialogueWords2[step])); // Start letter-by-letter display
                portraitImage2.sprite = portrait2[step];
                step++;
            }
        }
    }
    public void EnableNextLevelDoorTrigger()
    {
        // Enable the next level door trigger
        if (nextLevelDoor != null)
        {
            BoxCollider2D doorCollider = nextLevelDoor.GetComponent<BoxCollider2D>(); // Get the door's collider

            if (doorCollider != null)
            {
                doorCollider.isTrigger = true; // Ensure the collider is a trigger
                Debug.Log("nextLevelDoor collider enabled and set as trigger."); // Log success

                // Enable the SpriteRenderer to show the sprite
                if (doorSpriteRenderer != null)
                {
                    doorSpriteRenderer.enabled = true; // Show the sprite
                    Debug.Log("nextLevelDoor sprite renderer enabled."); // Log success
                }
                else
                {
                    Debug.LogWarning("No SpriteRenderer found on nextLevelDoor."); // Log warning if no SpriteRenderer
                }
            }
            else
            {
                Debug.LogWarning("No BoxCollider2D found on nextLevelDoor."); // Log warning if no collider
            }
        }
        else
        {
            Debug.LogWarning("nextLevelDoor reference is missing."); // Log warning if door reference is missing
        }
    }

    private IEnumerator DisplayDialogue(string dialogue)
    {
        isDisplaying = true; // Set the flag to true when starting the display
        dialogueText2.text = ""; // Clear previous text

        foreach (char letter in dialogue)
        {
            dialogueText2.text += letter; // Add one letter at a time
            yield return new WaitForSeconds(0.05f); // Adjust the speed here (0.05 seconds per letter)
        }

        isDisplaying = false; // Set the flag to false when finished displaying
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           
            dialogueActivated2 = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            // Check if the dialogueCanvas2 object still exists before accessing it
            if (dialogueCanvas2 != null && dialogueCanvas2.activeInHierarchy)
            {
                dialogueCanvas2.SetActive(false);
            }
            dialogueActivated2 = false;
            step = 0;
        }
    }
}
