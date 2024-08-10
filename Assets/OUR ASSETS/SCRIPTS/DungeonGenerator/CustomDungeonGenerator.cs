
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomDungeonGenerator : MonoBehaviour
{
    public static CustomDungeonGenerator Instance;

    [Header("ASSIGN VARIABLES")]
    //public GameObject[] roomPrefabs; // Assign different room prefabs in the Inspector

    public GameObject StartRoomPrefab;
    public GameObject BattleRoomPrefab;
    public GameObject BossRoomPrefab;
    public GameObject EmptyRoomPrefab;
    public GameObject ChestRoomPrefab;
    public GameObject EventRoomPrefab;
    public GameObject TrapRoomPrefab;
    public GameObject RestRoomPrefab;

    public float bossRoomChance = 0;
    public float emptyRoomChance = 0;
    public float chestRoomChance = 0;
    public float eventRoomChance = 0;
    public float trapRoomChance = 0;
    public float restRoomChance = 0;

    public GameObject linePrefab; // Assign in Inspector
    public Camera uiCamera; // Assign the camera used by the Canvas

    [Header("VALUE VARIABLES")]
    public int maxRoomLevels = 5; // Maximum number of room levels
    public int allowedPercentage = 30;
    public int distanceBetweenRooms = 2; // Distance you want to put between rooms
    //public int maxAttempts = 100; // Maximum attempts to create a room before stopping
    public bool hideRooms = true;
    public bool drawLines = false;
    public bool clickedStart = true;

    [Header("SCRIPT VARIABLES")]
    public List<GameObject> rooms = new List<GameObject>(); // List to store created rooms
    public Canvas canvas;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    //private Queue<GameObject> roomQueue = new Queue<GameObject>();
    private Dictionary<Vector2Int, GameObject> roomGrid = new Dictionary<Vector2Int, GameObject>();

    private Dictionary<GameObject, List<GameObject>> roomConnections = new Dictionary<GameObject, List<GameObject>>();
    private HashSet<(GameObject, GameObject)> existingConnections = new HashSet<(GameObject, GameObject)>();


    public int dungeonGeneratorPos = 0;
  

    public bool dungeonIsGenerating = false;
    public bool isBossRoomCreated = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {

        StartDungeonGeneration();
    }

    public void StartDungeonGeneration()
    {

        SystemManager.Instance.systemMode = SystemManager.SystemModes.DUNGEON;

        if (dungeonIsGenerating)
        {
            Debug.LogWarning("There is already a dungeon that is generating");
            return;
        }

        ValidateAssignments();
        try
        {
            StartCoroutine(GenerateDungeon());
        }
        catch(Exception ex)
        {
            Debug.LogWarning("ERROR : " + ex.Message);
        }
    }

    private void ValidateAssignments()
    {
        //canvas = GetComponentInChildren<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found. Please make sure the DungeonGenerator has a child Canvas.");
            return;
        }


        if (linePrefab == null)
        {
            Debug.LogError("Line Prefab not assigned.");
            return;
        }

        if (uiCamera == null)
        {
            Debug.LogError("UI Camera not assigned.");
            return;
        }

        if (eventSystem == null)
            eventSystem = FindObjectOfType<EventSystem>();

        if (raycaster == null)
            raycaster = FindObjectOfType<GraphicRaycaster>();

        if (uiCamera == null)
            uiCamera = FindObjectOfType<Camera>();

        if (eventSystem == null || raycaster == null || uiCamera == null)
        {
            Debug.LogError("EventSystem, GraphicRaycaster, or UI Camera not assigned or found in the scene.");
        }
        else
        {
            Debug.Log("EventSystem, GraphicRaycaster, and UI Camera are assigned correctly.");
        }
    }

    public IEnumerator GenerateDungeon()
    {
        //check the dungeon so it can not create another one until the current one is finished
        dungeonIsGenerating = true;
        //start generating
        yield return StartCoroutine(GenerateDungeonRoutine());
        //the dungeon is finished
        dungeonIsGenerating = false;
    }

    private IEnumerator GenerateDungeonRoutine()
    {

        //get the dungeon parent transform where every room will be child of
        Transform dungeonParentTransform = canvas.transform.Find("DungeonParent").transform;

        //initialize dungeon by clearing all remaining rooms from the parent
        yield return StartCoroutine(SystemManager.Instance.DestroyAllChildrenIE(dungeonParentTransform.gameObject));

        //clear the room gameobject list
        rooms.Clear();
        //clear the roomgrid that keeps track of all the rooms
        roomGrid.Clear();
        //clear the room connections
        roomConnections.Clear();

        //reset the pos of the dungeon generator
        dungeonGeneratorPos = 0;

        //start by first creating the starting room position which will then expand
        Vector2Int startRoomPosition = Vector2Int.zero;

        //reset rooms
        isBossRoomCreated = false;
   
        yield return StartCoroutine(CreateRoom(startRoomPosition, SystemManager.RoomType.Start));

        //start generating the dungeon by creating rooms
        while (rooms.Count < maxRoomLevels)
        {
     
            //generate rooms
            yield return StartCoroutine(GenerateLevel());
            //yield return null; // Wait for next frame to ensure all updates are applied

            //increase the position of the dungeon generator
            dungeonGeneratorPos += 1;

            //if the position is more than the current rooms then we just loop back to the beginning
            if (dungeonGeneratorPos >= rooms.Count)
            {
                dungeonGeneratorPos = 0;
            }

        }

        if (clickedStart)
        {
            GameObject startRoom = roomGrid[startRoomPosition];
            //click on the start
            startRoom.GetComponent<RoomScript>().ClickedRoom();
        }



    }

    public SystemManager.RoomType GetRoomType()
    {
        //default is the battle
        SystemManager.RoomType roomType = SystemManager.RoomType.Battle;
        float randomRoomChance = 0;

        //probability of the other rooms
        randomRoomChance = UnityEngine.Random.Range(0, 100);
        if ((randomRoomChance >= 0 && randomRoomChance <= bossRoomChance && isBossRoomCreated == false)
            || (isBossRoomCreated == false && rooms.Count == maxRoomLevels - 1))
        {
            roomType = SystemManager.RoomType.Boss;
            isBossRoomCreated = true;
            return roomType;
        }

        randomRoomChance = UnityEngine.Random.Range(0, 100);
        if (randomRoomChance >= 0 && randomRoomChance <= chestRoomChance)
        {
            roomType = SystemManager.RoomType.Chest;
            return roomType;
        }

        randomRoomChance = UnityEngine.Random.Range(0, 100);
        if (randomRoomChance >= 0 && randomRoomChance <= eventRoomChance)
        {
            roomType = SystemManager.RoomType.Event;
            return roomType;
        }
        
        if (randomRoomChance >= 0 && randomRoomChance <= trapRoomChance)
        {
            roomType = SystemManager.RoomType.Trap;
            return roomType;
        }

        randomRoomChance = UnityEngine.Random.Range(0, 100);
        if (randomRoomChance >= 0 && randomRoomChance <= restRoomChance)
        {
            roomType = SystemManager.RoomType.Rest;
            return roomType;
        }


        return roomType;


    }

    public IEnumerator GenerateLevel()
    {

        //then we get the latest room from the queue list so we can expand on it
        GameObject currentRoom = rooms[dungeonGeneratorPos];
        //then we start calculating the neigbor rooms
        yield return StartCoroutine(CreateNeighborRooms(currentRoom));
    }

    public GameObject GetRoomTypeGameObject(SystemManager.RoomType roomType)
    {

        if (roomType == SystemManager.RoomType.Start)
        {
            return StartRoomPrefab;
        }
        else if (roomType == SystemManager.RoomType.Battle)
        {
            return BattleRoomPrefab;
        }
        else if (roomType == SystemManager.RoomType.Boss)
        {
            return BossRoomPrefab;
        }
        else if (roomType == SystemManager.RoomType.Empty)
        {
            return EmptyRoomPrefab;
        }
        else if (roomType == SystemManager.RoomType.Chest)
        {
            return ChestRoomPrefab;
        }
        else if (roomType == SystemManager.RoomType.Event)
        {
            return EventRoomPrefab;
        }
        else if (roomType == SystemManager.RoomType.Trap)
        {
            return TrapRoomPrefab;
        }
        else if (roomType == SystemManager.RoomType.Rest)
        {
            return RestRoomPrefab;
        }

        return BattleRoomPrefab;

    }

    private IEnumerator CreateRoom(Vector2Int gridPosition, SystemManager.RoomType roomType)
    {
        Vector2 position = new Vector2(gridPosition.x * distanceBetweenRooms, gridPosition.y * distanceBetweenRooms);
        GameObject roomPrefab = GetRoomTypeGameObject(roomType);
        GameObject newRoom = Instantiate(roomPrefab, position, Quaternion.identity);
        RectTransform rt = newRoom.GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogError("Room Prefab does not have a RectTransform component.");
            yield break;
        }

        newRoom.transform.SetParent(canvas.transform.Find("DungeonParent").transform);
        newRoom.transform.localScale = new Vector3(1, 1, 1);
        
        //hide everything except the start room
        if (roomType != SystemManager.RoomType.Start && hideRooms == true) {
            newRoom.SetActive(false); // Set the room to be inactive initially
        }

        yield return null; // Wait for next frame to ensure the position is set

        TMP_Text roomText = newRoom.GetComponentInChildren<TMP_Text>();
        if (roomText != null)
        {
            roomText.text = ""; //roomType.ToString();
        }

        //add the room type
        newRoom.GetComponent<RoomScript>().roomType = roomType;

        //add the room to the list
        rooms.Add(newRoom);
        //add the room at the grid
        roomGrid[gridPosition] = newRoom;
        //add to the room connection
        roomConnections[newRoom] = new List<GameObject>();


    }

    public IEnumerator CreateNeighborRooms(GameObject room)
    {
        int randomPercentage = 0;

        //rooms can only spawn up,down,left and right from the current room
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        //we get the position of the current room as it will be used to calculate the neibour rooms
        Vector2Int roomGridPosition = GetRoomGridPosition(room);

        //loop through all the directions (up,down,left,right)
        foreach (Vector2Int dir in directions)
        {
           

            //if we reach the room limit then we stop
            if (rooms.Count >= maxRoomLevels)
            {
                Debug.Log("BREAK");
                break;
            }

            //calculate the position of the neighbor by using the direction
            Vector2Int adjacentGridPos = roomGridPosition + dir;

            // If there is already a room on that grid then we check for connection
            if (roomGrid.ContainsKey(adjacentGridPos))
            {
                GameObject adjacentRoom = roomGrid[adjacentGridPos];

                // Check if they are already connected
                if (roomConnections[room].Contains(adjacentRoom))
                {
                    continue; // If already connected, move to the next neighbor
                }

                // Random chance to connect the rooms
                randomPercentage = UnityEngine.Random.Range(0, 100);
                if (randomPercentage <= allowedPercentage)
                {
                    // Connect the rooms
              
                    ConnectRooms(room, adjacentRoom);
                }

                continue;
            }

            //the random percentage that it will generate a room if the position is empty . Meaning no room exist on that neighbor position
            randomPercentage = UnityEngine.Random.Range(0, 100);

            //if it is allowed based on our custom parameter
            if (randomPercentage <= allowedPercentage)
            {
       
                //then create room
                //get random room type
                SystemManager.RoomType roomType = GetRoomType();
                yield return StartCoroutine(CreateRoomAndConnect(room, adjacentGridPos, roomType));
            }
        }
    }

    private IEnumerator CreateRoomAndConnect(GameObject currentRoom, Vector2Int position, SystemManager.RoomType roomType)
    {
        //first we create the room
        yield return StartCoroutine(CreateRoom(position, roomType));
        //Get the room from the grid by using the position we would have spawn it
        GameObject newRoom = roomGrid[position];

        if (drawLines) {
            //then we draw a line between the current room and the neighbor created
            DrawLineBetweenRooms(currentRoom, newRoom);
        }

        // Add to the connections dictionary
        roomConnections[currentRoom].Add(newRoom);
        roomConnections[newRoom].Add(currentRoom);
    }

    private void ConnectRooms(GameObject room1, GameObject room2)
    {
        // Add to the connections dictionary
        roomConnections[room1].Add(room2);
        roomConnections[room2].Add(room1);

        // Draw a line between the rooms if drawing lines is enabled
        if (drawLines)
        {
            DrawLineBetweenRooms(room1, room2);
        }
    }
    private void DrawLineBetweenRooms(GameObject room1, GameObject room2)
    {
        // Ensure the pair is ordered to avoid duplicate entries (room1, room2) and (room2, room1)
        var connection = (room1, room2).Item1.GetInstanceID() < (room2, room1).Item1.GetInstanceID() ? (room1, room2) : (room2, room1);

        // Check if the connection already exists
        if (existingConnections.Contains(connection))
        {
            return; // Connection already exists, do not create a new line
        }

        // Add the connection to the set
        existingConnections.Add(connection);

        // We create a new line GameObject
        GameObject newLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        // We make it a child of the DungeonParent which holds also our rooms
        newLine.transform.SetParent(canvas.transform.Find("DungeonParent").transform, false);

        // Access the LineRenderer from the GameObject
        LineRenderer lineRenderer = newLine.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("Line Prefab does not have a LineRenderer component.");
            return;
        }

        // Calculate the LineRenderer position
        Vector3 startPosition = room1.transform.position;
        Vector3 endPosition = room2.transform.position;

        // And assign the positions
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }

    private Vector2Int GetRoomGridPosition(GameObject room)
    {
        //gets the room distance for the neighbours which will be used with direction
        Vector2 position = room.transform.position;
        return new Vector2Int(Mathf.RoundToInt(position.x / distanceBetweenRooms), Mathf.RoundToInt(position.y / distanceBetweenRooms));
    }

    //private void HideAllRoomsExceptStart()
    //{
    //    foreach (GameObject room in rooms)
    //    {
    //        if (room != rooms[0]) // Assuming the start room is the first one in the list
    //        {
    //            room.SetActive(false);
    //        }
    //    }
    //}

    public void OnRoomClick(GameObject clickedRoom)
    {
        if (roomConnections.ContainsKey(clickedRoom))
        {
            List<GameObject> connectedRooms = roomConnections[clickedRoom];
 
            foreach (GameObject room in connectedRooms)
            {
                // Here you can add code to highlight or show the connected rooms
       

                //make the room appear
                room.SetActive(true);

                //then we draw a line between the current room and the neighbor created
                DrawLineBetweenRooms(clickedRoom, room);

            }
        }
    }

}


