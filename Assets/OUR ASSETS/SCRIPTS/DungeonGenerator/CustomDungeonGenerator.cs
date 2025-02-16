
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
    public GameObject EliteBattlePlanetPrefab;
    public GameObject BossPlanetPrefab;
    public GameObject RewardPlanetPrefab;
    public GameObject EventPlanetPrefab;
    public GameObject RestPlanetPrefab;
    public GameObject ShopPlanetPrefab;
    public GameObject HiddenPlanetPrefab;

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

    private int howManyCombatRoom = 10;
    private int howManyEliteCombatRoom = 0;
    private int howManyBossRoom = 1;
    private int howManyRewardRoom = 1;
    private int howManyEventRoom = 6;
    private int howManyRestRoom = 1;
    private int howManyHiddenRoom = 0;
    private int howManyShopRoom = 0;

    private int maxRooms;

    private GameObject lastCreatedPlanet;
    List<GameObject> usedBossRooms = new List<GameObject>();

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
            ItemManager.Instance.ActivateItemList(SystemManager.ActivationType.OnAdventureSceneLoaded);
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
            ActivatePlanet(CombatManager.Instance.planetClicked);
            //CustomDungeonGenerator.Instance.OnRoomClick(CombatManager.Instance.planetClicked);
            //CombatManager.Instance.planetClicked.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = clearIcon;
            //CombatManager.Instance.planetClicked.GetComponent<RoomScript>().roomCleared = true;
            //CombatManager.Instance.planetClicked = null;
        }


    }

    public void ActivatePlanet(GameObject planet)
    {
        if (planet != null)
        {
            CustomDungeonGenerator.Instance.OnRoomClick(planet);
            planet.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = clearIcon;
            planet.GetComponent<RoomScript>().roomCleared = true;

        }

    }

    public void UpdateAdventureUI()
    {

        if (StaticData.staticCharacter == null)
        {
            StaticData.staticCharacter = CharacterManager.Instance.characterList[0].Clone();
        }

        if (StaticData.staticScriptableCompanion == null)
        {
            StaticData.staticScriptableCompanion = CharacterManager.Instance.companionList[0].Clone();
        }


        displayCharacterCard = GameObject.Find("ROOT").transform.Find("STAYONCAMERA").transform.Find("DisplayCharacterCard").gameObject;

        float targetValue = (float)StaticData.staticCharacter.currHealth / (float)StaticData.staticCharacter.maxHealth;
        displayCharacterCard.transform.Find("Health").GetComponent<Slider>().value = targetValue;

        displayCharacterCard.transform.Find("CardText").GetComponent<TMP_Text>().text = StaticData.staticCharacter.mainClass.ToString();

        displayCharacterCard.transform.Find("Health").Find("TEXT").GetComponent<TMP_Text>().text = StaticData.staticCharacter.currHealth + "/" + StaticData.staticCharacter.maxHealth;

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

        //assign the number of rooms to be generated
        howManyCombatRoom = UnityEngine.Random.Range(galaxyGenerating.minCombatRoom, galaxyGenerating.maxCombatRoom + 1);
        howManyEliteCombatRoom = UnityEngine.Random.Range(galaxyGenerating.minEliteCombatRoom, galaxyGenerating.maxEliteCombatRoom + 1);
        howManyBossRoom = UnityEngine.Random.Range(galaxyGenerating.minBossRoom, galaxyGenerating.maxBossRoom + 1);
        howManyRewardRoom = UnityEngine.Random.Range(galaxyGenerating.minRewardRoom, galaxyGenerating.maxRewardRoom + 1);
        howManyEventRoom = UnityEngine.Random.Range(galaxyGenerating.minEventRoom, galaxyGenerating.maxEventRoom + 1);
        howManyRestRoom = UnityEngine.Random.Range(galaxyGenerating.minRestRoom, galaxyGenerating.maxRestRoom + 1);
        howManyHiddenRoom = UnityEngine.Random.Range(galaxyGenerating.minHiddenRoom, galaxyGenerating.maxHiddenRoom + 1);
        howManyShopRoom = UnityEngine.Random.Range(galaxyGenerating.minShopRoom, galaxyGenerating.maxShopRoom + 1);

        //get the sum of all rooms as the max (wont count boss or hidden rooms as i want them to be on the edge of the galaxy
        maxRooms = howManyCombatRoom + howManyEliteCombatRoom + howManyRewardRoom + howManyEventRoom + howManyRestRoom + howManyShopRoom;
        Debug.Log("maxRooms : " + maxRooms);
    
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
        while (rooms.Count <= maxRooms || howManyBossRoom != 0 || howManyHiddenRoom != 0)
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

        //RULES OF DUNGEON

        //1ST RULE , THE FIRST ROOMS WILL BE STARTING / NOOBY FRIENDLY. AND WILL CONTAIN STARTING BATTLES/EVENTS ONLY
        if (rooms.Count < galaxyGenerating.startingRooms)
        {
            //if both rooms are available then choose randomly between them
            if (howManyCombatRoom != 0 && howManyEventRoom != 0)
            {
                int randomChance = UnityEngine.Random.Range(0, 1);

                if (randomChance == 0)
                {
                    howManyCombatRoom--;
                    planetType = SystemManager.PlanetTypes.STARTINGBATTLE;
                    return planetType;
                }
                else
                {
                    howManyEventRoom--;
                    planetType = SystemManager.PlanetTypes.EVENT;
                    return planetType;
                }
            }
            else if (howManyCombatRoom != 0)
            {
                howManyCombatRoom--;
                planetType = SystemManager.PlanetTypes.STARTINGBATTLE;
                return planetType;
            }
            else if (howManyEventRoom != 0)
            {
                howManyEventRoom--;
                planetType = SystemManager.PlanetTypes.EVENT;
                return planetType;
            }



        }

        //2ND RULE THE HALF ROOM WILL ALWAYS BE A REWARD ROOM
        if (rooms.Count == maxRooms / 2)
        {

            //if we have reward room available
            if (howManyRewardRoom != 0)
            {
                howManyRewardRoom--;
                planetType = SystemManager.PlanetTypes.REWARD;
                return planetType;
            }

        }


        List<SystemManager.PlanetTypes> planetTypes = new List<SystemManager.PlanetTypes>();
        planetTypes.Clear();

        //fill all other rooms
        if (howManyCombatRoom != 0)
        {
            planetTypes.Add(SystemManager.PlanetTypes.BATTLE);
        }

        if (howManyEliteCombatRoom != 0)
        {
            planetTypes.Add(SystemManager.PlanetTypes.ELITEBATTLE);
        }

        if (howManyEventRoom != 0)
        {
            planetTypes.Add(SystemManager.PlanetTypes.EVENT);
        }

        if (howManyRestRoom != 0)
        {
            planetTypes.Add(SystemManager.PlanetTypes.REST);
        }

        if (howManyRewardRoom != 0)
        {
            planetTypes.Add(SystemManager.PlanetTypes.REWARD);
        }

        if (howManyShopRoom != 0)
        {
            planetTypes.Add(SystemManager.PlanetTypes.SHOP);
        }

        if (planetTypes.Count > 0) {

            int randomIndex = UnityEngine.Random.Range(0,planetTypes.Count);
            SystemManager.PlanetTypes randomPlanetType = planetTypes[randomIndex];

            if (randomPlanetType == SystemManager.PlanetTypes.BATTLE)
            {
                howManyCombatRoom--;
            }
            else if (randomPlanetType == SystemManager.PlanetTypes.ELITEBATTLE)
            {
                howManyEliteCombatRoom--;
            }
            else if (randomPlanetType == SystemManager.PlanetTypes.EVENT)
            {
                howManyEventRoom--;
            }
            else if (randomPlanetType == SystemManager.PlanetTypes.REST)
            {
                howManyRestRoom--; 
            }
            else if (randomPlanetType == SystemManager.PlanetTypes.REWARD)
            {
                howManyRewardRoom--;
            }
            else if (randomPlanetType == SystemManager.PlanetTypes.SHOP)
            {
                howManyShopRoom--;
            }

            planetType = randomPlanetType;
        }

        return planetType;


    }

    public IEnumerator GenerateLevel()
    {

        //if we are done with the basic generated level then move to bosses and hidden room
        if (rooms.Count <= maxRooms)
        {
            //then we get the latest room from the queue list so we can expand on it
            GameObject currentRoom = rooms[dungeonGeneratorPos];
            //then we start calculating the neigbor rooms
            yield return StartCoroutine(CreateNeighborRooms(currentRoom));
        }
        else if (howManyBossRoom != 0)
        {
            howManyBossRoom--;
            //then generate boss logic

            //generate the rest point
            //GameObject furthestPlanet = GetFurthestRoomFromStartExcludeRestAndBoss();

            GameObject furthestPlanet = GetRandomRoomWithEmptyConection();

            usedBossRooms.Add(furthestPlanet);
            yield return StartCoroutine(CreateRoomsAfterGeneration(furthestPlanet, SystemManager.PlanetTypes.REST));

            //generate the boss on the rest point
            yield return StartCoroutine(CreateRoomsAfterGeneration(lastCreatedPlanet, SystemManager.PlanetTypes.BOSS));

        }
        else if (howManyHiddenRoom != 0)
        {
            howManyHiddenRoom--;
            //then generate hidden room logic

            GameObject furthestPlanet = GetRandomRoomWithOnlyOneConection();
            yield return StartCoroutine(CreateRoomsAfterGeneration(furthestPlanet, SystemManager.PlanetTypes.HIDDEN));
        }

    }

    public GameObject GetRoomTypeGameObject(SystemManager.PlanetTypes planetType)
    {

        if (planetType == SystemManager.PlanetTypes.START)
        {
            return StartPlanetPrefab;
        }
        else if (planetType == SystemManager.PlanetTypes.STARTINGBATTLE)
        {
            return BattlePlanetPrefab;
        }
        else if (planetType == SystemManager.PlanetTypes.BATTLE)
        {
            return BattlePlanetPrefab;
        }
        else if (planetType == SystemManager.PlanetTypes.ELITEBATTLE)
        {
            return EliteBattlePlanetPrefab;
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
        else if (planetType == SystemManager.PlanetTypes.SHOP)
        {
            return ShopPlanetPrefab;
        }
        else if (planetType == SystemManager.PlanetTypes.HIDDEN)
        {
            return HiddenPlanetPrefab;
        }

        return BattlePlanetPrefab;

    }

    private IEnumerator CreateRoom(Vector2Int gridPosition, SystemManager.PlanetTypes planetType)
    {
        Vector2 position;
        GameObject roomPrefab = GetRoomTypeGameObject(planetType);

        if (planetType != SystemManager.PlanetTypes.START)
        {
            position = new Vector2(gridPosition.x * galaxyGenerating.distanceBetweenRooms, gridPosition.y * galaxyGenerating.distanceBetweenRooms);
        }
        else
        {
            position = new Vector2(0, 0);
        }


        GameObject newRoom = Instantiate(roomPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);

        RectTransform rt = newRoom.GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogError("Room Prefab does not have a RectTransform component.");
            yield break;
        }

        newRoom.transform.SetParent(StaticData.staticDungeonParent.transform);
        newRoom.transform.localScale = new Vector3(1, 1, 1);
        //newRoom.transform.position = new Vector3(0, 0, 0);
        newRoom.transform.position = new Vector3(newRoom.transform.position.x, newRoom.transform.position.y, 0);

        if (planetType == SystemManager.PlanetTypes.BATTLE)
        {

            //get the planet
            int randomPlanetIndex = UnityEngine.Random.Range(0, galaxyGenerating.scriptablePlanets.Count);
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

    public IEnumerator CreateRoomsAfterGeneration(GameObject room, SystemManager.PlanetTypes planetType)
    {
        int randomPercentage = 0;

        //rooms can only spawn up,down,left and right from the current room
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        //we get the position of the current room as it will be used to calculate the neibour rooms
        Vector2Int roomGridPosition = GetRoomGridPosition(room);

        //loop through all the directions (up,down,left,right)
        foreach (Vector2Int dir in directions)
        {

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

                randomPercentage = UnityEngine.Random.Range(0, 100);
                if (randomPercentage <= galaxyGenerating.allowedPercentageConnectBoss && planetType == SystemManager.PlanetTypes.REST)
                {
                    // Connect the rooms

                    ConnectRooms(room, adjacentRoom);
                }

                continue;
            }


            //get random room type=
            yield return StartCoroutine(CreateRoomAndConnect(room, adjacentGridPos, planetType));
            break;

        }
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
            if (rooms.Count > maxRooms)
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
        lastCreatedPlanet = newRoom;

        if (planetType == SystemManager.PlanetTypes.BOSS || planetType == SystemManager.PlanetTypes.REST)
        {
            usedBossRooms.Add(newRoom);
        }
      
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

    public bool RoomConnectedTo(GameObject clickedRoom, SystemManager.PlanetTypes planetType)
    {
        bool connected = false;
        //check if the room is connected to the type specified

        if (roomConnections.ContainsKey(clickedRoom))
        {
            List<GameObject> connectedRooms = roomConnections[clickedRoom];

            foreach (GameObject room in connectedRooms)
            {

                if (room.GetComponent<RoomScript>().planetType == planetType)
                {
                    connected = true;
                    break;
                }

            }
        }

        return connected;
    }


    public GameObject GetFurthestRoomFromStart()
    {
        GameObject startRoom = rooms[0]; // Assuming the start room is the first room in the list
        Queue<GameObject> queue = new Queue<GameObject>();
        Dictionary<GameObject, int> distances = new Dictionary<GameObject, int>(); // To track the distance of each room from the start
        GameObject furthestRoom = startRoom;
        int maxDistance = 0;

        // Initialize BFS
        queue.Enqueue(startRoom);
        distances[startRoom] = 0; // Start room has a distance of 0

        while (queue.Count > 0)
        {
            GameObject currentRoom = queue.Dequeue();
            int currentDistance = distances[currentRoom];

            // Loop through all connected rooms
            foreach (GameObject neighbor in roomConnections[currentRoom])
            {
                if (!distances.ContainsKey(neighbor)) // If the neighbor hasn't been visited
                {
                    distances[neighbor] = currentDistance + 1; // Increase the distance by 1
                    queue.Enqueue(neighbor);

                    // Check if this is the furthest room found so far
                    if (distances[neighbor] > maxDistance)
                    {
                        maxDistance = distances[neighbor];
                        furthestRoom = neighbor;
                    }
                }
            }
        }

        return furthestRoom; // Return the furthest room
    }

    public GameObject GetFurthestRoomFromStartExcludeRest()
    {
        GameObject startRoom = rooms[0]; // Assuming the start room is the first room in the list
        Queue<GameObject> queue = new Queue<GameObject>();
        Dictionary<GameObject, int> distances = new Dictionary<GameObject, int>(); // To track the distance of each room from the start
        GameObject furthestRoom = startRoom;
        int maxDistance = 0;

        // Initialize BFS
        queue.Enqueue(startRoom);
        distances[startRoom] = 0; // Start room has a distance of 0

        while (queue.Count > 0)
        {
            GameObject currentRoom = queue.Dequeue();
            int currentDistance = distances[currentRoom];

            // Skip rooms with the planetType of REST or BOSS
            RoomScript roomScript = currentRoom.GetComponent<RoomScript>();
            if (roomScript != null && roomScript.planetType == SystemManager.PlanetTypes.REST
                && roomScript.planetType == SystemManager.PlanetTypes.BOSS)
            {
                continue; // Skip this room if its planetType is REST or BOSS
            }

            // Loop through all connected rooms
            foreach (GameObject neighbor in roomConnections[currentRoom])
            {
                if (!distances.ContainsKey(neighbor)) // If the neighbor hasn't been visited
                {
                    distances[neighbor] = currentDistance + 1; // Increase the distance by 1
                    queue.Enqueue(neighbor);

                    // Skip rooms with the planetType of REST
                    RoomScript neighborRoomScript = neighbor.GetComponent<RoomScript>();
                    if (roomScript != null && roomScript.planetType == SystemManager.PlanetTypes.REST
              && roomScript.planetType == SystemManager.PlanetTypes.BOSS)
                    {
                        continue; // Skip this room if its planetType is REST or BOSS
                    }

                    // Check if this is the furthest room found so far
                    if (distances[neighbor] > maxDistance)
                    {
                        maxDistance = distances[neighbor];
                        furthestRoom = neighbor;
                    }
                }
            }
        }

        return furthestRoom; // Return the furthest room that is not of planetType REST
    }

    public GameObject GetFurthestRoomFromStartExcludeRestAndBoss()
    {
        GameObject startRoom = rooms[0]; // Assuming the start room is the first room in the list
        Queue<GameObject> queue = new Queue<GameObject>();
        Dictionary<GameObject, int> distances = new Dictionary<GameObject, int>(); // To track the distance of each room from the start
        GameObject furthestRoom = startRoom;
        int maxDistance = 0;

        // Initialize BFS
        queue.Enqueue(startRoom);
        distances[startRoom] = 0; // Start room has a distance of 0

        while (queue.Count > 0)
        {
            GameObject currentRoom = queue.Dequeue();
            int currentDistance = distances[currentRoom];

            // Skip rooms with the planetType of REST or BOSS
            RoomScript roomScript = currentRoom.GetComponent<RoomScript>();
            if (roomScript != null && (roomScript.planetType == SystemManager.PlanetTypes.REST ||
                roomScript.planetType == SystemManager.PlanetTypes.BOSS))
            {
                continue; // Skip this room if its planetType is REST or BOSS
            }

            // Skip if the room has already been used as a boss room
            if (usedBossRooms.Contains(currentRoom))
            {
                continue;
            }

            // Loop through all connected rooms
            foreach (GameObject neighbor in roomConnections[currentRoom])
            {
                if (!distances.ContainsKey(neighbor)) // If the neighbor hasn't been visited
                {
                    distances[neighbor] = currentDistance + 1; // Increase the distance by 1
                    queue.Enqueue(neighbor);

                    // Skip rooms with the planetType of REST or BOSS
                    RoomScript neighborRoomScript = neighbor.GetComponent<RoomScript>();
                    if (neighborRoomScript != null && (neighborRoomScript.planetType == SystemManager.PlanetTypes.REST ||
                        neighborRoomScript.planetType == SystemManager.PlanetTypes.BOSS))
                    {
                        continue; // Skip this neighbor if its planetType is REST or BOSS
                    }

                    // Skip if the neighbor room has already been used as a boss room
                    if (usedBossRooms.Contains(neighbor))
                    {
                        continue;
                    }

                    // Check if this is the furthest room found so far
                    if (distances[neighbor] > maxDistance)
                    {
                        maxDistance = distances[neighbor];
                        furthestRoom = neighbor;
                    }
                }
            }
        }

        return furthestRoom; // Return the furthest room that is not of planetType REST or BOSS
    }

    public GameObject GetRandomRoomWithEmptyConection()
    {
        // Create a list to hold rooms that are only connected to one other room
        List<GameObject> roomsWithOneConnection = new List<GameObject>();

        // Loop through all rooms
        foreach (GameObject room in rooms)
        {

            if (usedBossRooms.Contains(room))
            {
                continue;
            }

            RoomScript roomFound = room.GetComponent<RoomScript>();

            if (roomFound.planetType == SystemManager.PlanetTypes.REST ||
                        roomFound.planetType == SystemManager.PlanetTypes.BOSS)
            {
                continue;
            }

            // Check how many neighbors (connections) this room has
            if (roomConnections.ContainsKey(room) && roomConnections[room].Count < 4)
            {
                // Add this room to the list if it is connected to exactly one other room
                roomsWithOneConnection.Add(room);
            }
        }

        // If there are no rooms with exactly one connection, return null
        if (roomsWithOneConnection.Count == 0)
        {
            return null;
        }

        // Randomly select a room from the list of rooms with one connection
        int randomIndex = UnityEngine.Random.Range(0, roomsWithOneConnection.Count);
        return roomsWithOneConnection[randomIndex];
    }


    public GameObject GetRandomRoomWithOnlyOneConection()
    {
        // Create a list to hold rooms that are only connected to one other room
        List<GameObject> roomsWithOneConnection = new List<GameObject>();

        // Loop through all rooms
        foreach (GameObject room in rooms)
        {

            if (usedBossRooms.Contains(room))
            {
                continue;
            }

            RoomScript roomFound = room.GetComponent<RoomScript>();

            if (roomFound.planetType == SystemManager.PlanetTypes.REST ||
                        roomFound.planetType == SystemManager.PlanetTypes.BOSS
                        || roomFound.planetType == SystemManager.PlanetTypes.HIDDEN)
            {
                continue;
            }

            // Check how many neighbors (connections) this room has
            if (roomConnections.ContainsKey(room) && roomConnections[room].Count == 1)
            {
                // Add this room to the list if it is connected to exactly one other room
                roomsWithOneConnection.Add(room);
            }
        }

        // If there are no rooms with exactly one connection, return null
        if (roomsWithOneConnection.Count == 0)
        {
            return null;
        }

        // Randomly select a room from the list of rooms with one connection
        int randomIndex = UnityEngine.Random.Range(0, roomsWithOneConnection.Count);
        return roomsWithOneConnection[randomIndex];
    }

}


