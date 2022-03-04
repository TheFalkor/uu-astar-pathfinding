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
    private const int ROW_COUNT = MAX_CAMERA_SIZE * 2;


    [Header("GameObject References")]
    private Camera cam;
    private readonly Node[] nodeArray = new Node[MAX_CAMERA_SIZE * MAX_CAMERA_SIZE * 4];
    private readonly List<Node> nodeList = new List<Node>();
    
    void Start()
    {
        cam = Camera.main;

        int minVisibleIndex = (MAX_CAMERA_SIZE - MIN_CAMERA_SIZE) * ROW_COUNT;
        int maxVisibleIndex = nodeArray.Length - minVisibleIndex - 1;
        for(int i = 0; i < nodeArray.Length; i++)
        {
            Node node = Instantiate(nodePrefab, new Vector2(-MAX_CAMERA_SIZE + 0.5f + i % ROW_COUNT, MAX_CAMERA_SIZE - 0.5f - i / ROW_COUNT), Quaternion.identity).GetComponent<Node>();
            if (i < minVisibleIndex || i > maxVisibleIndex)
                node.HideNode();
            else
            {
                if (i % ROW_COUNT < MAX_CAMERA_SIZE - MIN_CAMERA_SIZE)
                    node.HideNode();
                else if (i % ROW_COUNT > ROW_COUNT - MAX_CAMERA_SIZE + MIN_CAMERA_SIZE - 1)
                    node.HideNode();
            }

            nodeArray[i] = node;
        }
    }

    
    void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if(hit)
            {
                hit.transform.GetComponent<Node>().ToggleNode();
            }
        }

        if(Input.mouseScrollDelta.y < 0)
        {
            IncreaseGridSize();
        }
        else if(Input.mouseScrollDelta.y > 0)
        {
            DecreaseGridSize();
        }
    }

    private void IncreaseGridSize()
    {
        if (cam.orthographicSize >= MAX_CAMERA_SIZE)
            return;

        cam.orthographicSize += 1;
        int size = (int)cam.orthographicSize * 2;
        int count = size * 2 + (size - 2) * 2;

        int startIndex = (MAX_CAMERA_SIZE - (int)cam.orthographicSize) * MAX_CAMERA_SIZE * 2 + MAX_CAMERA_SIZE - (int)cam.orthographicSize;
        
        for (int i = 0; i < count; i++)
        {
            if (i < size)
            {
                nodeArray[startIndex + i].ShowNode();
            }
            else if(i < count - size)
            {
                int index = startIndex + ROW_COUNT * ((i - size) / 2 + 1) + (i + 1) % 2 * (size - 1);
                nodeArray[index].ShowNode();
            }
            else
            {
                int index = startIndex + (size -1)* ROW_COUNT + (i - count + size);
                nodeArray[index].ShowNode();
            }
        }
    }

    private void DecreaseGridSize()
    {
        if (cam.orthographicSize <= MIN_CAMERA_SIZE)
            return;

        int size = (int)cam.orthographicSize * 2;
        int count = size * 2 + (size - 2) * 2;
        int startIndex = (MAX_CAMERA_SIZE - (int)cam.orthographicSize) * MAX_CAMERA_SIZE * 2 + MAX_CAMERA_SIZE - (int)cam.orthographicSize;

        cam.orthographicSize -= 1;

        for (int i = 0; i < count; i++)
        {
            if (i < size)
            {
                nodeArray[startIndex + i].HideNode();
            }
            else if (i < count - size)
            {
                int index = startIndex + ROW_COUNT * ((i - size) / 2 + 1) + (i + 1) % 2 * (size - 1);
                nodeArray[index].HideNode();
            }
            else
            {
                int index = startIndex + (size - 1) * ROW_COUNT + (i - count + size);
                nodeArray[index].HideNode();
            }
        }
    }
}
