using UnityEngine;

public class WaitingRoomTrigger : MonoBehaviour
{
    public RoomGenerator generator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            generator.OnReachedWaitingRoom();
        }
    }
}