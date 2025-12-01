using UnityEngine;
using System.Collections;

public class WaitingRoomTrigger : MonoBehaviour
{
    public RoomGenerator generator;
    public GameTimer timer;

    public float waitTimer = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (timer != null)
            {
                timer.isRunning = false;
            }

            generator.OnReachedWaitingRoom();
            StartCoroutine(WaitAndAllowProceed());
        }
    }

    private IEnumerator WaitAndAllowProceed()
    {
        Debug.Log("Waiting room entered. Waiting for " + waitTimer + " seconds...");
        yield return new WaitForSeconds(waitTimer);
        DoorTrigger.canProceed = true;
        Debug.Log("Wait over! Player can now proceed through the waiting room door.");
    }
}