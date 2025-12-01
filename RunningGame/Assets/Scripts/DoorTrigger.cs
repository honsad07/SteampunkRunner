using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public GameObject doorToDisable;
    public bool isWaitingRoomDoor = false;
    public RoomGenerator generator;

    // This defaults to FALSE when the game loads. This is correct.
    public static bool canProceed = false; 

    private GameTimer timer;
    private bool triggered = false;

    // ⛔ REMOVE THE AWAKE() METHOD ⛔
    /*
    private void Awake()
    {
        timer = FindObjectOfType<GameTimer>();
        if (timer == null)
        {
            Debug.LogError("DoorTrigger could not find GameTimer in the scene!");
        }
        
        // This line was causing the bug. Remove it.
        // DoorTrigger.canProceed = true; 
    }
    */
    
    // 💡 NEW: Add a Start() method just to find the timer.
    // We use Start() instead of Awake() to avoid potential timing issues.
    private void Start()
    {
        timer = FindObjectOfType<GameTimer>();
        if (timer == null)
        {
            Debug.LogError("DoorTrigger could not find GameTimer in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || triggered) return;

        // This logic is now correct.
        if (isWaitingRoomDoor)
        {
            if (!canProceed)
            {
                Debug.Log("Waiting period not over. Cannot proceed yet. Wait for the countdown.");
                return; // ⬅️ This will now work.
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
            
            // This correctly locks the *next* waiting room door.
            canProceed = false; 
        }
    }
}