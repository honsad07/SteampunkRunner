using UnityEngine;

public class WaitingRoomDetector : MonoBehaviour
{
    public RoomGenerator generator;

    void Update()
    {
        if (generator.currentEndWaitingRoom == null) return;

        Collider waitingRoomCollider = generator.currentEndWaitingRoom.GetComponent<Collider>();
        if (waitingRoomCollider == null) return;

        // Check if player is inside the waiting room
        if (waitingRoomCollider.bounds.Contains(transform.position))
        {
            generator.OnReachedWaitingRoom();
        }
    }
}