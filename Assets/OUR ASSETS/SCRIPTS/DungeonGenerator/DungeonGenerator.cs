using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject roomButtonPrefab; // Assign in Inspector
    public GameObject linePrefab; // Assign in Inspector
    public int gridSize = 5; // Size of the grid (total rooms will be gridSize x gridSize)
    public float roomSpacing = 300f; // Spacing between rooms in the grid
    public Camera uiCamera; // Assign the camera used by the Canvas
    public float minRoomDistance = 250f; // Minimum distance between rooms
    public int maxRoomLevels = 5; // Maximum number of room levels
    public int roomsPerLevel = 5; // Number of rooms to generate per level
    public List<GameObject> rooms = new List<GameObject>(); // List to store created rooms

    private Canvas canvas;

    void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        canvas = GetComponentInChildren<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found. Please make sure the DungeonGenerator has a child Canvas.");
            return;
        }

        if (roomButtonPrefab == null)
        {
            Debug.LogError("Room Button Prefab not assigned.");
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

        StartCoroutine(GenerateDungeonRoutine());
    }

    private IEnumerator GenerateDungeonRoutine()
    {
        Transform dungeonParentTransform = canvas.transform.Find("DungeonParent").transform;
        yield return StartCoroutine(SystemManager.Instance.DestroyAllChildrenIE(dungeonParentTransform.gameObject));

        rooms.Clear();

        // Place start room
        Vector2 startRoomPosition = new Vector2(roomSpacing, roomSpacing); // Example: Start room at (1, 1)
        CreateRoom(startRoomPosition, "Start");

        // Generate dungeon levels
        for (int level = 1; level <= maxRoomLevels; level++)
        {
            GenerateLevel(startRoomPosition, level);
        }

        ConnectRooms();
    }

    void GenerateLevel(Vector2 startRoomPosition, int level)
    {
        List<Vector2> roomPositions = new List<Vector2>();
        roomPositions.Add(startRoomPosition);

        // Determine how many rooms to generate for this level
        int roomsToGenerate = Mathf.Min(level * roomsPerLevel, gridSize * gridSize - 1); // -1 for excluding start room

        for (int i = 0; i < roomsToGenerate; i++)
        {
            Vector2 newRoomPosition = GetRandomAdjacentRoomPosition(roomPositions);
            if (newRoomPosition == Vector2.zero)
            {
                continue; // No valid position found, skip this iteration
            }

            roomPositions.Add(newRoomPosition);
            CreateRoom(newRoomPosition, $"Room Level {level}-{i + 1}");
        }
    }

    Vector2 GetRandomAdjacentRoomPosition(List<Vector2> existingRoomPositions)
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        Shuffle(directions);

        foreach (Vector2 dir in directions)
        {
            Vector2 potentialPosition = existingRoomPositions[Random.Range(0, existingRoomPositions.Count)] + dir * roomSpacing;

            if (!IsPositionOccupied(potentialPosition) && IsInGridBounds(potentialPosition))
            {
                return potentialPosition;
            }
        }

        return Vector2.zero; // No valid position found
    }

    bool IsPositionOccupied(Vector2 position)
    {
        foreach (GameObject room in rooms)
        {
            RectTransform rt = room.GetComponent<RectTransform>();
            if (Vector2.Distance(rt.anchoredPosition, position) < minRoomDistance)
            {
                return true;
            }
        }
        return false;
    }

    bool IsInGridBounds(Vector2 position)
    {
        float maxPosition = (gridSize - 1) * roomSpacing / 2;
        return Mathf.Abs(position.x) <= maxPosition && Mathf.Abs(position.y) <= maxPosition;
    }

    void CreateRoom(Vector2 position, string roomName)
    {
        GameObject newRoom = Instantiate(roomButtonPrefab, canvas.transform.Find("DungeonParent").transform);
        RectTransform rt = newRoom.GetComponent<RectTransform>();
        if (rt == null)
        {
            Debug.LogError("Room Button Prefab does not have a RectTransform component.");
            return;
        }

        // Convert position to canvas local coordinates
        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, position, uiCamera, out canvasPos);
        rt.anchoredPosition = canvasPos;

        TMP_Text roomText = newRoom.GetComponentInChildren<TMP_Text>();
        if (roomText != null)
        {
            roomText.text = roomName;
        }

        rooms.Add(newRoom);
    }

    void ConnectRooms()
    {
        Dictionary<Vector2Int, List<Vector2Int>> roomConnections = new Dictionary<Vector2Int, List<Vector2Int>>();

        // Initialize roomConnections dictionary
        foreach (GameObject room in rooms)
        {
            Vector2Int roomGridPos = WorldToGridPosition(room.GetComponent<RectTransform>().anchoredPosition);
            if (!roomConnections.ContainsKey(roomGridPos))
            {
                roomConnections[roomGridPos] = new List<Vector2Int>();
            }
        }

        foreach (GameObject room in rooms)
        {
            RectTransform rt = room.GetComponent<RectTransform>();
            Vector2Int roomGridPos = WorldToGridPosition(rt.anchoredPosition);

            ConnectAdjacentRooms(roomGridPos, roomConnections);
        }

        EnsureAllRoomsConnected(roomConnections);
    }

    void ConnectAdjacentRooms(Vector2Int roomGridPos, Dictionary<Vector2Int, List<Vector2Int>> roomConnections)
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int adjacentPos = roomGridPos + dir;
            if (IsValidGridPosition(adjacentPos))
            {
                GameObject adjacentRoom = FindRoomAtGridPosition(adjacentPos);
                if (adjacentRoom != null)
                {
                    if (!roomConnections.ContainsKey(adjacentPos))
                    {
                        roomConnections[adjacentPos] = new List<Vector2Int>();
                    }

                    roomConnections[roomGridPos].Add(adjacentPos);
                    roomConnections[adjacentPos].Add(roomGridPos);

                    ConnectRoomsAtPositions(FindRoomAtGridPosition(roomGridPos).GetComponent<RectTransform>(), adjacentRoom.GetComponent<RectTransform>());
                }
            }
        }
    }

    void EnsureAllRoomsConnected(Dictionary<Vector2Int, List<Vector2Int>> roomConnections)
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        Vector2Int startRoomGridPos = WorldToGridPosition(rooms[0].GetComponent<RectTransform>().anchoredPosition);
        queue.Enqueue(startRoomGridPos);
        visited.Add(startRoomGridPos);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int neighbor in roomConnections[current])
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        foreach (GameObject room in rooms)
        {
            Vector2Int roomGridPos = WorldToGridPosition(room.GetComponent<RectTransform>().anchoredPosition);
            if (!visited.Contains(roomGridPos))
            {
                Vector2Int nearestConnectedRoom = GetNearestConnectedRoom(roomGridPos, visited);
                roomConnections[roomGridPos].Add(nearestConnectedRoom);
                roomConnections[nearestConnectedRoom].Add(roomGridPos);

                ConnectRoomsAtPositions(FindRoomAtGridPosition(roomGridPos).GetComponent<RectTransform>(), FindRoomAtGridPosition(nearestConnectedRoom).GetComponent<RectTransform>());
            }
        }
    }

    Vector2Int GetNearestConnectedRoom(Vector2Int roomGridPos, HashSet<Vector2Int> visited)
    {
        Vector2Int nearest = visited.GetEnumerator().Current;
        float minDistance = float.MaxValue;

        foreach (Vector2Int pos in visited)
        {
            float distance = Vector2Int.Distance(roomGridPos, pos);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = pos;
            }
        }

        return nearest;
    }

    void ConnectRoomsAtPositions(RectTransform room1, RectTransform room2)
    {
        Vector3 startWorldPos = ConvertCanvasToWorldPosition(room1);
        Vector3 endWorldPos = ConvertCanvasToWorldPosition(room2);
        DrawLine(startWorldPos, endWorldPos);
    }

    Vector3 ConvertCanvasToWorldPosition(RectTransform rectTransform)
    {
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);
        return (worldCorners[0] + worldCorners[2]) / 2;
    }

    Vector2Int WorldToGridPosition(Vector2 worldPosition)
    {
        int col = Mathf.RoundToInt(worldPosition.x / roomSpacing);
        int row = Mathf.RoundToInt(worldPosition.y / roomSpacing);
        return new Vector2Int(col, row);
    }

    GameObject FindRoomAtGridPosition(Vector2Int gridPosition)
    {
        foreach (GameObject room in rooms)
        {
            RectTransform rt = room.GetComponent<RectTransform>();
            Vector2Int roomGridPos = WorldToGridPosition(rt.anchoredPosition);
            if (roomGridPos == gridPosition)
            {
                return room;
            }
        }
        return null;
    }

    bool IsValidGridPosition(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < gridSize && gridPosition.y >= 0 && gridPosition.y < gridSize;
    }

    void DrawLine(Vector3 start, Vector3 end)
    {
        GameObject line = Instantiate(linePrefab, canvas.transform.Find("DungeonParent").transform);
        LineRenderer lr = line.GetComponent<LineRenderer>();
        if (lr == null)
        {
            Debug.LogError("Line Prefab does not have a LineRenderer component.");
            return;
        }

        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }

    void Shuffle(Vector2[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Vector2 temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    void Shuffle(List<GameObject> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            GameObject temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}