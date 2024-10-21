
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomDungeonGenerator : MonoBehaviour
{
    public static CustomDungeonGenerator Instance;

    [Header("ASSIGN VARIABLES")]
    //public GameObject[] roomPrefabs; // Assign different room prefabs in the Inspector

    public int galaxyLevel = 1;

    public List<ScriptableGalaxies> scriptableGalaxies_1;
    public List<ScriptableGalaxies> scriptableGalaxies_2;
    public List<ScriptableGalaxies> scriptableGalaxies_3;
    public List<ScriptableGalaxies> scriptableGalaxies_4; //end game
    public List<ScriptableGalaxies> scriptableGalaxies_5; //secret

    public ScriptableGalaxies galaxyGenerating;

    public Sprite clearIcon;

    public GameObject StartPlanetPrefab;
    public GameObject BattlePlanetPrefab;
    public GameObject BossPlanetPrefab;
    public GameObject RewardPlanetPrefab;
    public GameObject EventPlanetPrefab;
    public GameObject RestPlanetPrefab;


    public GameObject linePrefab; // Assign in Inspector
    public Camera uiCamera; // Assign the camera used by the Canvas

    [Header("VALUE VARIABLES")]

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


    //ui
    public GameObject displayCharacterCard;


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





    }


    void OnEnable()
    {

  

        // Register the callback when the object is enabled
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unregister the callback when the object is disabled
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This function will be called whenever a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        // Call your function here

        CustomSceneLoaded();


    }

    public void CustomSceneLoaded()
    {

        Scene scene = SceneManager.GetActiveScene();

        if (scene.name != "scene_Adventure")
        {
            return;
        }
        else
        {
            UpdateAdventureUI();
        }

        //first check if dungeonParent is null or not
        if (StaticData.staticDungeonParent != null && StaticData.staticDungeonParentGenerated == true)
        {
            StaticData.staticDungeonParent.SetActive(true);
        }
        else
        {
            StaticData.staticDungeonParent = canvas.transform.GetChild(0).gameObject;
            StaticData.staticDungeonParent.SetActive(true);
            StartDungeonGeneration();
        }

        //if came back from battle
        if (CombatManager.Instance.planetClicked != null)
        {
            //activate the neighbours
            CustomDungeonGenerator.Instance.OnRoomClick(CombatManager.Instance.planetClicked);
            CombatManager.Instance.planetClicked.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = clearIcon;
            CombatManager.Instance.planetClicked.GetComponent<RoomScript>().roomCleared = true;
            //CombatManager.Instance.planetClicked = null;
        }


    }

    public void UpdateAdventureUI()
    {

        if (StaticData.staticCharacter == null)
        {
            StaticData.staticCharacter = CharacterManager.Instance.characterList[0].Clone();
        }


        displayCharacterCard = GameObject.Find("ROOT").transform.Find("STAYONCAMERA").transform.Find("DisplayCharacterCard").gameObject;

        float targetValue = (float)StaticData.staticCharacter.currHealth / (float)StaticData.staticCharacter.maxHealth;
        displayCharacterCard.transform.Find("Health").GetComponent<Slider>().value = targetValue;

        displayCharacterCard.transform.Find("CardText").GetComponent<TMP_Text>().text = StaticData.staticCharacter.mainClass.ToString();

        displayCharacterCard.transform.Find("Health").Find("TEXT").GetComponent<TMP_Text>().text = StaticData.staticCharacter.currHealth + " / " + StaticData.staticCharacter.maxHealth;

        displayCharacterCard.transform.Find("EntityImage").GetComponent<Image>().sprite = StaticData.staticCharacter.entityImage;

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
            StaticData.staticDungeonParentGenerated = true;
            StartCoroutine(GenerateDungeon());
        }
        catch (Exception ex)
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

        //if (uiCamera == null)
        //{
        //    uiCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //    Debug.LogError("UI Camera not assigned.");
        //    return;
        //}

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

        yield return StartCoroutine(GetAppropriateGalaxy());

        //start generating
        yield return StartCoroutine(GenerateDungeonRoutine());
        //the dungeon is finished
        dungeonIsGenerating = false;
    }

    public IEnumerator GetAppropriateGalaxy()
    {

        int randomIndex = 0;

        if (galaxyLevel == 1)
        {
            randomIndex = UnityEngine.Random.Range(0, scriptableGalaxies_1.Count);
            galaxyGenerating = scriptableGalaxies_1[randomIndex];
        }
        else if (galaxyLevel == 2)
        {
            randomIndex = UnityEngine.Random.Range(0, scriptableGalaxies_2.Count);
            galaxyGenerating = scriptableGalaxies_2[randomIndex];
        }
        else if (galaxyLevel == 3)
        {
            randomIndex = UnityEngine.Random.Range(0, scriptableGalaxies_3.Count);
            galaxyGenerating = scriptableGalaxies_3[randomIndex];
        }
        else if (galaxyLevel == 4)
        {
            randomIndex = UnityEngine.Random.Range(0, scriptableGalaxies_4.Count);
            galaxyGenerating = scriptableGalaxies_4[randomIndex];
        }
        else if (galaxyLevel == 5)
        {
            randomIndex = UnityEngine.Random.Range(0, scriptableGalaxies_5.Count);
            galaxyGenerating = scriptableGalaxies_5[randomIndex];
        }


        yield return null;
    }

    private IEnumerator GenerateDungeonRoutine()
    {


        if (galaxyGenerating == null)
        {
            dungeonIsGenerating = false;
            yield return null;
        }

        //get the dungeon parent transform where every room will be child of

        Transform dungeonParentTransform = StaticData.staticDungeonParent.transform;

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

        yield return StartCoroutine(CreateRoom(startRoomPosition, SystemManager.PlanetTypes.START));

        //start generating the dungeon by creating rooms
        while (rooms.Count < galaxyGenerating.maxRoomLevels)
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

    public SystemManager.PlanetTypes GetPlanetType()
    {
        //default is the battle
        SystemManager.PlanetTypes planetType = SystemManager.PlanetTypes.BATTLE;
        float randomRoomChance = 0;

        //probability of the other rooms
        randomRoomChance = UnityEngine.Random.Range(0, 100);
        if ((randomRoomChance >= 0 && randomRoomChance <= galaxyGenerating.bossRoomChance && isBossRoomCreated == false)
            || (isBossRoomCreated == false && rooms.Count == galaxyGenerating.maxRoomLevels - 1))
        {
            planetType = SystemManager.PlanetTypes.BOSS;
            isBossRoomCreated = true;
            return planetType;
        }

        randomRoomChance = UnityEngine.Random.Range(0, 100);
        if (randomRoomChance >= 0 && randomRoomChance <= galaxyGenerating.rewardRoomChance)
        {
            planetType = SystemManager.PlanetTypes.REWARD;
            return planetType;
        }

        randomRoomChance = UnityEngine.Random.Range(0, 100);
        if (randomRoomChance >= 0 && randomRoomChance <= galaxyGenerating.eventRoomChance)
        {
            planetType = SystemManager.PlanetTypes.EVENT;
            return planetType;
        }

        randomRoomChance = UnityEngine.Random.Range(0, 100);
        if (randomRoomChance >= 0 && randomRoomChance <= galaxyGenerating.restRoomChance)
        {
            planetType = SystemManager.PlanetTypes.REST;
            return planetType;
        }


        return planetType;


    }

    public IEnumerator GenerateLevel()
    {

        //then we get the latest room from the queue list so we can expand on it
        GameObject currentRoom = rooms[dungeonGeneratorPos];
        //then we start calculating the neigbor rooms
        yield return StartCoroutine(CreateNeighborRooms(currentRoom));
    }

    public GameObject GetRoomTypeGameObject(SystemManager.PlanetTypes planetType)
    {

        if (planetType == SystemManager.PlanetTypes.START)
        {
            return StartPlanetPrefab;
        }
        else if (planetType == SystemManager.PlanetTypes.BATTLE)
        {
            return BattlePlanetPrefab;
        }
        else if (planetType == SystemManager.PlanetTypes.BOSS)
        {
            return BossPlanetPrefab;
        }
        else if (planetType == SystemManager.PlanetTypes.REWARD)
        {
            return RewardPlanetPrefab;
        }
        else if (planetType == SystemManager.PlanetTypes.EVENT)
        {
            return EventPlanetPrefab;
        }
        else if (planetType == SystemManager.PlanetTypes.REST)
        {
            return RestPlanetPrefab;
        }

        return BattlePlanetPrefab;

    }

    private IEnumerator CreateRoom(Vector2Int gridPosition, SystemManager.PlanetTypes planetType)
    {
        Vector2 position = new Vector2(gridPosition.x * galaxyGenerating.distanceBetweenRooms, gridPosition.y * galaxyGenerating.distanceBetweenRooms);
        GameObject roomPrefab = GetRoomTypeGameObject(planetType);
        GameObject newRoom = Instantiate(roomPrefab,new Vector3(position.x,position.y,0), Quaternion.identity);
        RectTransform rt = newRoom.GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogError("Room Prefab does not have a RectTransform component.");
            yield break;
        }

        newRoom.transform.SetParent(StaticData.staticDungeonParent.transform);
        newRoom.transform.localScale = new Vector3(1, 1, 1);
        newRoom.transform.position = new Vector3(newRoom.transform.position.x, newRoom.transform.position.y,0);

        if (planetType == SystemManager.PlanetTypes.BATTLE)
        {

            //get the planet
            int randomPlanetIndex = UnityEngine.Random.Range(0,galaxyGenerating.scriptablePlanets.Count);
            ScriptablePlanets scriptablePlanet = galaxyGenerating.scriptablePlanets[randomPlanetIndex];
            newRoom.GetComponent<RoomScript>().scriptablePlanet = scriptablePlanet;

            newRoom.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = scriptablePlanet.planetArt;

        }

        //hide everything except the start room
        if (planetType != SystemManager.PlanetTypes.START && hideRooms == true)
        {
            newRoom.SetActive(false); // Set the room to be inactive initially
        }

        yield return null; // Wait for next frame to ensure the position is set

        TMP_Text roomText = newRoom.GetComponentInChildren<TMP_Text>();
        if (roomText != null)
        {
            roomText.text = ""; //roomType.ToString();
        }

        //add the room type
        newRoom.GetComponent<RoomScript>().planetType = planetType;

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
            if (rooms.Count >= galaxyGenerating.maxRoomLevels)
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
                if (randomPercentage <= galaxyGenerating.allowedPercentage)
                {
                    // Connect the rooms

                    ConnectRooms(room, adjacentRoom);
                }

                continue;
            }

            //the random percentage that it will generate a room if the position is empty . Meaning no room exist on that neighbor position
            randomPercentage = UnityEngine.Random.Range(0, 100);

            //if it is allowed based on our custom parameter
            if (randomPercentage <= galaxyGenerating.allowedPercentage)
            {

                //then create room
                //get random room type
                SystemManager.PlanetTypes planetType = GetPlanetType();
                yield return StartCoroutine(CreateRoomAndConnect(room, adjacentGridPos, planetType));
            }
        }
    }

    private IEnumerator CreateRoomAndConnect(GameObject currentRoom, Vector2Int position, SystemManager.PlanetTypes planetType)
    {
        //first we create the room
        yield return StartCoroutine(CreateRoom(position, planetType));
        //Get the room from the grid by using the position we would have spawn it
        GameObject newRoom = roomGrid[position];

        if (drawLines)
        {
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
        newLine.transform.SetParent(StaticData.staticDungeonParent.transform, false);

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
        return new Vector2Int(Mathf.RoundToInt(position.x / galaxyGenerating.distanceBetweenRooms), Mathf.RoundToInt(position.y / galaxyGenerating.distanceBetweenRooms));
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

        if (clickedRoom.GetComponent<RoomScript>().planetType != SystemManager.PlanetTypes.START)
        {
            clickedRoom.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = CustomDungeonGenerator.Instance.clearIcon;
        }

        clickedRoom.GetComponent<RoomScript>().roomCleared = true;

    }

}


