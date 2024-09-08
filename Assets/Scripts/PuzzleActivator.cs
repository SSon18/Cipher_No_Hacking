using UnityEngine;

public class PuzzleActivator : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Find the PuzzleSystem and activate the puzzle
            PuzzleSystem puzzleSystem = FindObjectOfType<PuzzleSystem>();
            if (puzzleSystem != null)
            {
                puzzleSystem.ActivatePuzzle();
            }
        }
    }
}
