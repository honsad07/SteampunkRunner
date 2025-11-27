using UnityEngine;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour
{
    private int roomCount = 3;
    private int level = 1;

    public List<GameObject> straightRooms;
    public List<GameObject> leftRooms;
    public List<GameObject> rightRooms;
    public GameObject startEndRoom;
    
    public GameObject previousEndWaitingRoom;
    public GameObject currentEndWaitingRoom;

    private Transform currentEXIT;
    private Direction currentDirection = Direction.Forward;
    private bool lastRoomWasTurn = false;

    private enum Direction {Forward, Left, Right };

    void Start()
    {
        GenerateRooms();
    }

    void GenerateRooms()
    {
        GameObject newLevelParent = new GameObject("Level_" + level);

        Vector3 startPosition;
        Quaternion startRotation;

        if (currentEndWaitingRoom != null)
        {
            RoomHandler prevExitHandler = currentEndWaitingRoom.GetComponent<RoomHandler>();
        
            currentEXIT = prevExitHandler.exit;
        }
        else
        {
            startPosition = Vector3.zero;
            startRotation = Quaternion.identity;

            GameObject startRoom = Instantiate(startEndRoom, startPosition, startRotation);

            currentEXIT = startRoom.GetComponent<RoomHandler>().exit;
        }

        for (int i = 0; i < roomCount; i++)
        {
            GameObject prefab = ChooseNextRoom();
            GameObject room = Instantiate(prefab, currentEXIT.position, currentEXIT.rotation);
            room.transform.parent = newLevelParent.transform;
            currentEXIT = room.GetComponent<RoomHandler>().exit;
        }

        GameObject newEndWaitingRoom = Instantiate(startEndRoom, currentEXIT.position, currentEXIT.rotation);
        var trigger = newEndWaitingRoom.AddComponent<WaitingRoomTrigger>();
        trigger.generator = this;

        if (currentEndWaitingRoom != null) previousEndWaitingRoom = currentEndWaitingRoom;

        currentEndWaitingRoom = newEndWaitingRoom;

        level++;
        roomCount += 5;
    }

    GameObject ChooseNextRoom()
    {
        if(lastRoomWasTurn)
        {
            lastRoomWasTurn = false;
            return straightRooms[Random.Range(0, straightRooms.Count)];
        }

        float picker = Random.value;

        if(picker < 0.6f )
        {
            return straightRooms[Random.Range(0, straightRooms.Count)];
        }
        else
        {
            bool willTurnLeft = Random.value < 0.5f;

            if(willTurnLeft && currentDirection == Direction.Forward)
            {
                currentDirection = Direction.Left;
                lastRoomWasTurn = true;
                return leftRooms[Random.Range(0, leftRooms.Count)];
            }
            else if(willTurnLeft && currentDirection == Direction.Right)
            {
                currentDirection = Direction.Forward;
                lastRoomWasTurn = true;
                return leftRooms[Random.Range(0, leftRooms.Count)];
            }
            else if(!willTurnLeft && currentDirection == Direction.Forward)
            {
                currentDirection = Direction.Right;
                lastRoomWasTurn = true;
                return rightRooms[Random.Range(0, rightRooms.Count)];
            }
            else if(!willTurnLeft && currentDirection == Direction.Left)
            {
                currentDirection = Direction.Forward;
                lastRoomWasTurn = true;
                return rightRooms[Random.Range(0, rightRooms.Count)];
            }
            else
            {
                return straightRooms[Random.Range(0, straightRooms.Count)];
            }
        }
    }

    public void OnReachedWaitingRoom()
    {
        string oldLevelName = "Level_" + (level - 1);

        GameObject oldLevel = GameObject.Find(oldLevelName);

        if (oldLevel != null)
        {
            Destroy(oldLevel);
            Debug.Log("Destroyed: " + oldLevelName);
        }
        else
        {
            Debug.Log("Could not find: " + oldLevelName);
        }

        if (previousEndWaitingRoom != null)
        {
            Destroy(previousEndWaitingRoom);
            previousEndWaitingRoom = null;
        }

        Transform enterDoor = currentEndWaitingRoom.transform.Find("Doors/Enter");
        if (enterDoor != null)
        {
            enterDoor.gameObject.SetActive(true);
            Debug.Log("Closed entrance door.");
        }
        else
        {
            Debug.LogWarning("Enter door not found! Check path: Doors/Enter");
        }

        GenerateRooms();
    }
}