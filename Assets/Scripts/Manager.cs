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
    private const KeyCode KEY_FILLNODES = KeyCode.F;
    private const KeyCode KEY_RANDOMENTITY = KeyCode.R;
    [Space]
    private const KeyCode KEY_SELECT_SPACESHIP = KeyCode.Alpha1;
    private const KeyCode KEY_SELECT_TRADINGPOST = KeyCode.Alpha2;
    private const KeyCode KEY_SELECT_FALLENSTAR = KeyCode.Alpha3;
    private const KeyCode KEY_SELECT_STARCHASER = KeyCode.Alpha4;


    [Header("Manager Variables")]
    private const int ROW_COUNT = MAX_CAMERA_SIZE * 2;
    private readonly Node[] nodeArray = new Node[ROW_COUNT * ROW_COUNT];
    private readonly List<Node> visibleNodeList = new List<Node>();
    private int currentCameraSize;
    private bool leftClicking = false;
    private bool rightClicking = false;
    [Space]
    public GameObject selectedEntity;


    [Header("Object References")]
    private Camera cam;
    private Transform nodeParentTransform;
    [Space]
    private GameObject spaceship;
    private GameObject tradingPost;
    private GameObject fallenStar;
    private GameObject starchaser;

    public static Manager instance;
    

    void Start()
    {
        if (instance == null)
            instance = this;

        cam = Camera.main;
        currentCameraSize = (int)cam.orthographicSize;
        nodeParentTransform = new GameObject("Nodes").transform;

        spaceship = GameObject.Find("Spaceship").gameObject;
        tradingPost = GameObject.Find("TradingPost").gameObject;
        fallenStar = GameObject.Find("FallenStar").gameObject;
        starchaser = GameObject.Find("Starchaser").gameObject;
        
        int minVisibleIndex = (MAX_CAMERA_SIZE - MIN_CAMERA_SIZE) * ROW_COUNT;
        int maxVisibleIndex = nodeArray.Length - minVisibleIndex - 1;
        for(int i = 0; i < nodeArray.Length; i++)
        {
            Node node = Instantiate(nodePrefab, new Vector2(-MAX_CAMERA_SIZE + i % ROW_COUNT, MAX_CAMERA_SIZE - i / ROW_COUNT), Quaternion.identity, nodeParentTransform).GetComponent<Node>();
            if (i > minVisibleIndex && i < maxVisibleIndex)
            {
                if (i % ROW_COUNT >= MAX_CAMERA_SIZE - MIN_CAMERA_SIZE && i % ROW_COUNT < ROW_COUNT - MAX_CAMERA_SIZE + MIN_CAMERA_SIZE)
                    node.SetNodeVisible(true);
                else
                    node.SetNodeVisible(false);
            }
            else
                node.SetNodeVisible(false);

            nodeArray[i] = node;
        }


        SetEntityPosition(spaceship);
        SetEntityPosition(tradingPost);
        SetEntityPosition(fallenStar);
        SetEntityPosition(starchaser);
    }

    
    void Update()
    {
        leftClicking = !rightClicking && Input.GetMouseButton(0);
        rightClicking = !leftClicking && Input.GetMouseButton(1);

        if(leftClicking)
        {
            Vector2Int mousePosition = Vector2Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if(selectedEntity)
            {
                if (SetEntityPosition(selectedEntity, mousePosition.x, mousePosition.y))
                    selectedEntity = null;
            }
            else
            {
                ToggleNode(mousePosition, true);
            }
        }
        else if(rightClicking)
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

        if (Input.GetKeyUp(KEY_RESETNODES))
        {
            for (int i = 0; i < nodeArray.Length; i++)
                nodeArray[i].SetNodeBlocked(false);
        }

        if (Input.GetKeyUp(KEY_FILLNODES))
        {
            for (int i = 0; i < nodeArray.Length; i++)
                nodeArray[i].SetNodeBlocked(true);
        }

        if(Input.GetKeyUp(KEY_RANDOMENTITY))
        {
            SetEntityPosition(spaceship);
            SetEntityPosition(tradingPost);
            SetEntityPosition(fallenStar);
            SetEntityPosition(starchaser);
        }

        if (Input.GetKeyUp(KEY_SELECT_SPACESHIP))
            selectedEntity = spaceship;
        if (Input.GetKeyUp(KEY_SELECT_TRADINGPOST))
            selectedEntity = tradingPost;
        if (Input.GetKeyUp(KEY_SELECT_FALLENSTAR))
            selectedEntity = fallenStar;
        if (Input.GetKeyUp(KEY_SELECT_STARCHASER))
            selectedEntity = starchaser;
    }


    public void RemoveNode(Node node)
    {
        visibleNodeList.Remove(node);
    }

    public void AddNode(Node node)
    {
        visibleNodeList.Add(node);
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


    private void SetEntityPosition(GameObject entity)
    {
        List<Node> emptyNodes = new List<Node>();

        for (int i = 0; i < visibleNodeList.Count; i++)
        {
            if (!visibleNodeList[i].blocked && !visibleNodeList[i].GetBlocked())
                emptyNodes.Add(visibleNodeList[i]);
        }
        
        if(emptyNodes.Count > 0)
        { 
            int prevIndex = ((int)entity.transform.position.x + MAX_CAMERA_SIZE) % ROW_COUNT + (MAX_CAMERA_SIZE - (int)entity.transform.position.y) * ROW_COUNT;
            int newIndex = Random.Range(0, emptyNodes.Count);
            
            nodeArray[prevIndex].SetOccupied(null);
            entity.transform.position = emptyNodes[newIndex].transform.position;
            emptyNodes[newIndex].SetOccupied(entity);

            entity.SetActive(true);
        }
    }

    private bool SetEntityPosition(GameObject entity, int x, int y)
    {
        int prevIndex = ((int)entity.transform.position.x + MAX_CAMERA_SIZE) % ROW_COUNT + (MAX_CAMERA_SIZE - (int)entity.transform.position.y) * ROW_COUNT;
        int newIndex = (x + MAX_CAMERA_SIZE) % ROW_COUNT + (MAX_CAMERA_SIZE - y) * ROW_COUNT;

        if (nodeArray[newIndex].GetBlocked())
            return false;

        nodeArray[prevIndex].SetOccupied(null);
        nodeArray[newIndex].SetOccupied(entity);

        entity.transform.position = new Vector2(x, y);
        entity.SetActive(true);

        return true;
    }
}
