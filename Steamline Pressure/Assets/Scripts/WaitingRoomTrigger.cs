using UnityEngine;
using System.Collections;

public class WaitingRoomTrigger : MonoBehaviour
{
    private Score scoreScript;
    public RoomGenerator generator;
    public GameTimer timer;

    public float waitTimer = 5f;

    private void Start()
    {
        scoreScript = FindFirstObjectByType<Score>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (timer != null)
            {
                timer.isRunning = false;
                scoreScript.newScore += 99 - (int)Mathf.Floor(timer.timeRemaining);
                scoreScript.newScore *= scoreScript.multiplier;
                scoreScript.multiplier = 1;
                scoreScript.score += scoreScript.newScore;
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