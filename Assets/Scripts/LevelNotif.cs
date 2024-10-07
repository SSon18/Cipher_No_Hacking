using UnityEngine;
using TMPro;
using System.Collections;

public class PopupNotification : MonoBehaviour
{
    [SerializeField] private GameObject notificationPanel; // Reference to your prefab panel
    [SerializeField] private TextMeshProUGUI notificationText; // Reference to the TextMeshPro component
    [SerializeField] private float displayDuration = 5f; // Time in seconds to show the notification
    [SerializeField] private string sceneName; // Text to display, editable in the inspector

    private void Start()
    {
        // Set the scene name (can be done manually or dynamically)
        if (string.IsNullOrEmpty(sceneName))
        {
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name; // Automatically set to current scene's name
        }

        notificationText.text = sceneName; // Set the text on the notification panel
        notificationPanel.SetActive(true); // Show the panel

        // Start the coroutine to hide the notification after a certain time
        StartCoroutine(HideNotification());
    }

    private IEnumerator HideNotification()
    {
        yield return new WaitForSeconds(displayDuration); // Wait for the specified time
        notificationPanel.SetActive(false); // Hide the panel
    }
}
