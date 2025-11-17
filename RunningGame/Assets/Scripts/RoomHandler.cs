using UnityEngine;

public class RoomHandler : MonoBehaviour
{
    public Transform exit;

    void Awake()
    {
        exit = transform.Find("EXIT");
    }
}
