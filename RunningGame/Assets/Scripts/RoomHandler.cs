using UnityEngine;

public class RoomHandler : MonoBehaviour
{
    public Transform exit;

    void Awake()
    {
        exit = transform.Find("Doors/Exit/EXIT");
        if(exit == null)
        {
            exit = transform.Find("Door/Exit/EXIT");
        }
    }
}
