using UnityEngine;

public class PayphoneTrigger : MonoBehaviour
{
    public PuzzleUIManager puzzleUI;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            puzzleUI.ShowPuzzle();
        }
    }
}