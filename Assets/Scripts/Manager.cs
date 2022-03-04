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

    private KeyCode KEY_CLEARNODES = KeyCode.C;


    [Header("GameObject References")]
    private Camera cam;
    private readonly Node[] nodeArray = new Node[MAX_CAMERA_SIZE * MAX_CAMERA_SIZE * 4];
    

    

    void Start()
    {
        cam = Camera.main;

        int minVisibleIndex = (MAX_CAMERA_SIZE - MIN_CAMERA_SIZE) * ROW_COUNT;
        int maxVisibleIndex = nodeArray.Length - minVisibleIndex - 1;
        for(int i = 0; i < nodeArray.Length; i++)
        {
            Node node = Instantiate(nodePrefab, new Vector2(-MAX_CAMERA_SIZE + 0.5f + i % ROW_COUNT, MAX_CAMERA_SIZE - 0.5f - i / ROW_COUNT), Quaternion.identity).GetComponent<Node>();
            if (i < minVisibleIndex || i > maxVisibleIndex)
                node.ToggleNodeVisible();
            else
            {
                if (i % ROW_COUNT < MAX_CAMERA_SIZE - MIN_CAMERA_SIZE)
                    node.ToggleNodeVisible();
                else if (i % ROW_COUNT > ROW_COUNT - MAX_CAMERA_SIZE + MIN_CAMERA_SIZE - 1)
                    node.ToggleNodeVisible();
            }

            nodeArray[i] = node;
        }
    }

    
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if(hit)
            {
                hit.transform.GetComponent<Node>().SetNodeBlocked(true);
            }
        }
        else if(Input.GetMouseButton(1))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit)
            {
                hit.transform.GetComponent<Node>().SetNodeBlocked(false);
            }
        }

        if(Input.mouseScrollDelta.y < 0)
        {
            ChangeGridSize(1);
        }
        else if(Input.mouseScrollDelta.y > 0)
        {
            ChangeGridSize(-1);
        }

        if(Input.GetKeyUp(KEY_CLEARNODES))
        {
            for (int i = 0; i < nodeArray.Length; i++)
                nodeArray[i].SetNodeBlocked(false);
        }
    }

    private void ChangeGridSize(int direction)
    {
        if (cam.orthographicSize + direction > MAX_CAMERA_SIZE || cam.orthographicSize + direction < MIN_CAMERA_SIZE)
            return;

        if(direction > 0)
            cam.orthographicSize += 1;

        int size = (int)cam.orthographicSize * 2;
        int count = size * 2 + (size - 2) * 2;

        int startIndex = (MAX_CAMERA_SIZE - (int)cam.orthographicSize) * MAX_CAMERA_SIZE * 2 + MAX_CAMERA_SIZE - (int)cam.orthographicSize;
        
        for (int i = 0; i < count; i++)
        {
            if (i < size)
            {
                nodeArray[startIndex + i].ToggleNodeVisible();
            }
            else if(i < count - size)
            {
                int index = startIndex + ROW_COUNT * ((i - size) / 2 + 1) + (i + 1) % 2 * (size - 1);
                nodeArray[index].ToggleNodeVisible();
            }
            else
            {
                int index = startIndex + (size -1)* ROW_COUNT + (i - count + size);
                nodeArray[index].ToggleNodeVisible();
            }
        }

        if (direction < 0)
            cam.orthographicSize -= 1;
    }
}
