using UnityEngine;

public class CoinTrigger : MonoBehaviour
{
    private Score scoreScript;

    private void Start()
    {
        scoreScript = FindFirstObjectByType<Score>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            scoreScript.multiplier += 0.1f;
            Destroy(gameObject);
        }
    }
}
