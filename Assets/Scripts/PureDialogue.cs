using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
    private bool isDisplaying;

    // Reference to PlayerMovement and Animator
    [SerializeField]
    private NewBehaviourScript playerMovement;

    private Animator playerAnimator;
    private SpriteRenderer notifRenderer;
    private GameObject notif;

    [SerializeField]
    private GameObject interactable;

    [SerializeField]
    private GameObject nextLevelDoor; // Object for the door leading to the next level
    private BoxCollider2D doorCollider;
    private SpriteRenderer doorSpriteRenderer;

    private string doorSaveKey;

    private void Start()
    {
        notifRenderer = interactable.GetComponent<SpriteRenderer>();
        if (notifRenderer != null)
        {
            notifRenderer.enabled = false; // Hide the sprite initially
        }

        doorCollider = nextLevelDoor.GetComponent<BoxCollider2D>();
        doorSpriteRenderer = nextLevelDoor.GetComponent<SpriteRenderer>();

        if (doorSpriteRenderer != null)
        {
            doorSpriteRenderer.enabled = false; // Hide the sprite initially
        }

        // Generate a unique key based on the door's name for saving its state
        doorSaveKey = nextLevelDoor.name + "_isTrigger";

        // Load the door's state
        LoadDoorState();

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
        if (Input.GetButtonDown("Talk") && dialogueActivated2 && !isDisplaying)
        {
            if (step >= speaker2.Length)
            {
                dialogueCanvas2.SetActive(false);
                step = 0;
                dialogueActivated2 = false;

                notifRenderer.enabled = false;

                // Re-enable player movement and animation
                if (playerMovement != null)
                {
                    playerMovement.enabled = true;
                }

                if (playerAnimator != null)
                {
                    playerAnimator.SetBool("run", true);
                }

                EnableNextLevelDoorTrigger();
            }
            else
            {
                dialogueCanvas2.SetActive(true);
                speakerText2.text = speaker2[step];
                StartCoroutine(DisplayDialogue(dialogueWords2[step]));
                portraitImage2.sprite = portrait2[step];
                step++;
                playerMovement.enabled = false;
                playerMovement.body.velocity = Vector2.zero;
            }
        }
    }

    public void EnableNextLevelDoorTrigger()
    {
        if (doorCollider != null)
        {
            doorCollider.isTrigger = true; // Enable door trigger

            // Enable the SpriteRenderer to show the door
            if (doorSpriteRenderer != null)
            {
                doorSpriteRenderer.enabled = true;
            }

            // Save the door's state
            PlayerPrefs.SetInt(doorSaveKey, doorCollider.isTrigger ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    private void LoadDoorState()
    {
        if (doorCollider != null)
        {
            // Load the saved state for the door's trigger
            bool isTrigger = PlayerPrefs.GetInt(doorSaveKey, 0) == 1;
            doorCollider.isTrigger = isTrigger;

            // Show or hide the door based on the saved state
            if (doorSpriteRenderer != null)
            {
                doorSpriteRenderer.enabled = isTrigger;
            }
        }
    }

    private IEnumerator DisplayDialogue(string dialogue)
    {
        isDisplaying = true;
        dialogueText2.text = "";

        foreach (char letter in dialogue)
        {
            dialogueText2.text += letter;
            yield return new WaitForSeconds(0.02f);
        }

        isDisplaying = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dialogueActivated2 = true;
            notifRenderer.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (dialogueCanvas2 != null && dialogueCanvas2.activeInHierarchy)
            {
                dialogueCanvas2.SetActive(false);
            }
            dialogueActivated2 = false;
            step = 0;
            notifRenderer.enabled = false;
        }
    }

    public void closeDialogueBTN()
    {
        dialogueCanvas2.SetActive(false);
        playerMovement.enabled = true;
    }
}
