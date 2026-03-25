using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            FirebaseManager.Instance.AddCoin(1);
            Destroy(gameObject);
        }
    }
}