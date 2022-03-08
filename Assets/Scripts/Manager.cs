using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [Header("Inspector References")]
    [SerializeField] private GameObject nodePrefab;


    [Header("Settings")]
    private const int START_SIZE = 5;
    private const KeyCode KEY_RESETNODES = KeyCode.C;
    private const KeyCode KEY_FILLNODES = KeyCode.F;
    private const KeyCode KEY_RANDOMENTITY = KeyCode.R;
    private const KeyCode KEY_NOISEPATTERN = KeyCode.Q;
    private const KeyCode KEY_STARTALGORITHM = KeyCode.Space;


    [Header("Manager Variables")]
    private bool leftClicking = false;
    private bool rightClicking = false;
    private Entity currentEntity;


    [Header("Object References")]
    private Camera cam;
    private Entity spaceship;
    private Entity tradingPost;
    private Entity fallenStar;
    private Entity starchaser;


    [Header("Public References")]
    public static Manager instance;
    public GridController grid;
    

    void Start()
    {
        if (instance == null)
            instance = this;


        cam = Camera.main;
        cam.orthographicSize = START_SIZE;
        
        spaceship = GameObject.Find("Spaceship").GetComponent<Entity>();
        tradingPost = GameObject.Find("TradingPost").GetComponent<Entity>();
        fallenStar = GameObject.Find("FallenStar").GetComponent<Entity>();
        starchaser = GameObject.Find("Starchaser").GetComponent<Entity>();

        grid = new GridController(nodePrefab, START_SIZE);

        grid.RandomizeEntityPosition(spaceship);
        grid.RandomizeEntityPosition(tradingPost);
        grid.RandomizeEntityPosition(fallenStar);
        grid.RandomizeEntityPosition(starchaser);
    }


    void Update()
    {
        leftClicking = !rightClicking && Input.GetMouseButton(0);
        rightClicking = !leftClicking && Input.GetMouseButton(1);
        Vector2Int mousePosition = Vector2Int.RoundToInt(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if(leftClicking && !currentEntity)
        {
            grid.SetNodeBlocked(mousePosition, true);
        }
        else if(rightClicking && !currentEntity)
        {
            grid.SetNodeBlocked(mousePosition, false);
        }
        else if(Input.GetMouseButtonUp(0))
        {
            if (currentEntity)
                grid.SetEntityPosition(currentEntity, mousePosition);
            else
                SelectEntity(grid.GetEntity(mousePosition));
        }
        else if(Input.GetMouseButtonUp(1))
        {
            SelectEntity(null);
        }

        if(Input.mouseScrollDelta.y < 0)
        {
            if (grid.ChangeGridSize(1))
                cam.orthographicSize += 1;
        }
        else if(Input.mouseScrollDelta.y > 0)
        {
            if (grid.ChangeGridSize(-1))
                cam.orthographicSize -= 1;
        }

        if (Input.GetKeyUp(KEY_RESETNODES))
        {
            grid.ResetNodes();
        }

        if (Input.GetKeyUp(KEY_FILLNODES))
        {
            grid.FillNodes();
        }

        if (Input.GetKeyUp(KEY_NOISEPATTERN))
        {
            grid.NoisePattern();
        }

        if (Input.GetKeyUp(KEY_RANDOMENTITY))
        {
            grid.RandomizeEntityPosition(spaceship);
            grid.RandomizeEntityPosition(tradingPost);
            grid.RandomizeEntityPosition(fallenStar);
            grid.RandomizeEntityPosition(starchaser);
            SelectEntity(null);
        }

        if(Input.GetKeyUp(KEY_STARTALGORITHM))   // Just for testing
        {
            starchaser.GetComponent<Starchaser>().Test(grid.GetNode(starchaser.GetPosition()), grid.GetNode(fallenStar.GetPosition()));
        }    
    }


    public void SelectEntity(Entity entity)
    {
        if (currentEntity)
            currentEntity.SetSelected(false);

        if (currentEntity == entity)
        {
            currentEntity = null;
            return;
        }

        currentEntity = entity;

        if (currentEntity)
            currentEntity.SetSelected(true);
    }
}
