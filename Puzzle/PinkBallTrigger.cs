using UnityEngine;

public class PinkBallTrigger : MonoBehaviour
{
    public PuzzleUIManager puzzleManager;
    public GameObject greenBall; // assign dari Inspector

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == greenBall)
        {
            puzzleManager.PuzzleSuccess();
        }
    }
}