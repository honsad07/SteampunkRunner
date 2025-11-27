using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Tooltip("Door object to disable when the player enters the trigger.")]
    public GameObject doorToDisable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (doorToDisable != null)
            {
                doorToDisable.SetActive(false);
                // Debug.Log("Door disabled");
            }
        }
    }
}