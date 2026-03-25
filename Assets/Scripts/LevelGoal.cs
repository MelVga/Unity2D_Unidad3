using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    public int levelNumber = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            FirebaseManager.Instance.CompleteLevel(levelNumber);
            Debug.Log("Nivel completado: " + levelNumber);
        }
    }
}