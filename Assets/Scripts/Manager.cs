using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [Header("Inspector References")]
    [SerializeField] private GameObject nodePrefab;


    [Header("Settings")]
    private const int MIN_CAMERA_SIZE = 5;
    private const int MAX_CAMERA_SIZE = 15;
    private const KeyCode KEY_RESETNODES = KeyCode.C;


    [Header("Manager Variables")]
    private const int ROW_COUNT = MAX_CAMERA_SIZE * 2;
    private readonly Node[] nodeArray = new Node[ROW_COUNT * ROW_COUNT];
    private int currentCameraSize;


    [Header("Object References")]
    private Camera cam;
    private Transform nodeParentTransform;
    

    void Start()
    {
        cam = Camera.main;
        currentCameraSize = (int)cam.orthographicSize;
        nodeParentTransform = new GameObject("Nodes").transform;
        
        int minVisibleIndex = (MAX_CAMERA_SIZE - MIN_CAMERA_SIZE) * ROW_COUNT;
        int maxVisibleIndex = nodeArray.Length - minVisibleIndex - 1;
        for(int i = 0; i < nodeArray.Length; i++)
        {
            Node node = Instantiate(nodePrefab, new Vector2(-MAX_CAMERA_SIZE + i % ROW_COUNT, MAX_CAMERA_SIZE - i / ROW_COUNT), Quaternion.identity, nodeParentTransform).GetComponent<Node>();
            if (i < minVisibleIndex || i > maxVisibleIndex)
                node.ToggleNodeVisible();
            else
            {
                if (i % ROW_COUNT < MAX_CAMERA_SIZE - MIN_CAMERA_SIZE)
                    node.ToggleNodeVisible();
                else if (i % ROW_COUNT + 1 > ROW_COUNT - MAX_CAMERA_SIZE + MIN_CAMERA_SIZE)
                    node.ToggleNodeVisible();
            }

            nodeArray[i] = node;
        }
    }

 
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Vector2Int mousePosition = Vector2Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            ToggleNode(mousePosition, true);
        }
        else if(Input.GetMouseButton(1))
        {
            Vector2Int mousePosition = Vector2Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            ToggleNode(mousePosition, false);
        }

        if(Input.mouseScrollDelta.y < 0)
        {
            ChangeGridSize(1);
        }
        else if(Input.mouseScrollDelta.y > 0)
        {
            ChangeGridSize(-1);
        }

        if(Input.GetKeyUp(KEY_RESETNODES))
        {
            for (int i = 0; i < nodeArray.Length; i++)
                nodeArray[i].SetNodeBlocked(false);
        }
    }


    private void ToggleNode(Vector2Int position, bool blocked)
    {
        if (position.x < -currentCameraSize || position.x >= currentCameraSize || position.y <= -currentCameraSize || position.y > currentCameraSize)
            return;

        int index = (position.x + MAX_CAMERA_SIZE) % ROW_COUNT + (MAX_CAMERA_SIZE - position.y) * ROW_COUNT;
        nodeArray[index].SetNodeBlocked(blocked);
    }


    private void ChangeGridSize(int direction)
    {
        if (currentCameraSize + direction > MAX_CAMERA_SIZE || currentCameraSize + direction < MIN_CAMERA_SIZE)
            return;

        if(direction > 0)
            currentCameraSize += 1;


        int sideLength = currentCameraSize * 2 - 1;
        int columnCount = sideLength * 2;

        int startIndex = (MAX_CAMERA_SIZE - currentCameraSize) * ROW_COUNT + MAX_CAMERA_SIZE - currentCameraSize;

        for (int i = 0; i <= sideLength; i++)    // Top row
            nodeArray[startIndex + i].ToggleNodeVisible();

        for (int i = 2; i < columnCount; i++)   // Columns
            nodeArray[startIndex + (i / 2) * ROW_COUNT + (i + 1) % 2 * sideLength].ToggleNodeVisible();

        for (int i = 0; i <= sideLength; i++)    // Bottom row
            nodeArray[startIndex + sideLength * ROW_COUNT + i].ToggleNodeVisible();


        if (direction < 0)
            currentCameraSize -= 1;

        cam.orthographicSize = currentCameraSize;
    }
}
