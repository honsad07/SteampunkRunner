using UnityEngine;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour
{
    int roomCount = 8;
    int level = 1;

    public List<GameObject> straightRooms;
    public List<GameObject> leftRooms;
    public List<GameObject> rightRooms;
    public GameObject startEndRoom;

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
        GameObject start = Instantiate(startEndRoom, Vector3.zero, Quaternion.identity);
        currentEXIT = start.GetComponent<RoomHandler>().exit;

        for(int i = 0; i < roomCount; i++)
        {
            SpawnNextRoom();
        }

        Instantiate(startEndRoom, currentEXIT.position, currentEXIT.rotation);
    }

    void SpawnNextRoom()
    {
        GameObject prefab = ChooseNextRoom();
        GameObject room = Instantiate(prefab, currentEXIT.position, currentEXIT.rotation);
        currentEXIT = room.GetComponent<RoomHandler>().exit;
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
}
