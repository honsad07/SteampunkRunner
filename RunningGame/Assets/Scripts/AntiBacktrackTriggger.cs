using UnityEngine;

public class AntiBacktrackTrigger : MonoBehaviour
{
    [Tooltip("Door that should be activated when player steps back.")]
    public GameObject exitDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (exitDoor != null)
            exitDoor.SetActive(true);
    }
}