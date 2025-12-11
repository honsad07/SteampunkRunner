using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private Score scoreScript;

    public GameObject doorToDisable;
    public bool isWaitingRoomDoor = false;
    public RoomGenerator generator;

    public static bool canProceed = false; 

    private GameTimer timer;
    private bool triggered = false;

    private void Start()
    {
        timer = FindFirstObjectByType<GameTimer>();
        scoreScript = FindFirstObjectByType<Score>();
        if (timer == null)
        {
            Debug.LogError("DoorTrigger could not find GameTimer in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || triggered) return;

        if (isWaitingRoomDoor)
        {
            if (!canProceed)
            {
                Debug.Log("Waiting period not over. Cannot proceed yet. Wait for the countdown.");
                return;
            }
        }
        
        triggered = true;

        if (doorToDisable != null) doorToDisable.SetActive(false);

        if (isWaitingRoomDoor)
        {
            if (timer != null)
            {
                timer.ResetTimer(99.999f);
                timer.isRunning = true;
                Debug.Log("Timer started upon exiting waiting room.");
            }
            
            canProceed = false; 
        }
    }
}