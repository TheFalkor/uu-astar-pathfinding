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
    private const KeyCode KEY_STARTSIMULATION = KeyCode.Space;


    [Header("Manager Variables")]
    private bool leftClicking = false;
    private bool rightClicking = false;
    private bool simulationActive = false;
    private Entity currentEntity;


    [Header("Object References")]
    private Camera cam;
    private Entity spaceship;
    private Entity tradingPost;
    private Entity star;
    private Starchaser starchaser;


    [Header("Public References")]
    public static Manager instance;
    public GridController grid;
    private UIController uiController;
    

    void Awake()
    {
        if (instance == null)
            instance = this;
        

        cam = Camera.main;
        cam.orthographicSize = START_SIZE;
        
        spaceship = GameObject.Find("Spaceship").GetComponent<Entity>();
        tradingPost = GameObject.Find("TradingPost").GetComponent<Entity>();
        star = GameObject.Find("FallenStar").GetComponent<Entity>();
        starchaser = GameObject.Find("Starchaser").GetComponent<Starchaser>();

        starchaser.SetReferences(spaceship, tradingPost, star);

        grid = new GridController(nodePrefab, START_SIZE);
        uiController = GameObject.Find("Canvas").GetComponent<UIController>();

        RandomizeEntityPositions();
    }


    void Update()
    {
        if(Input.GetKeyUp(KEY_STARTSIMULATION))
        {
            ToggleSimulation();
        }    

        if(simulationActive)
        {
            starchaser.UpdateSimulation(Time.deltaTime);
            return;
        }
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


        if (Input.GetKeyUp(KEY_RESETNODES))
        {
            uiController.GridClear();
        }

        if (Input.GetKeyUp(KEY_FILLNODES))
        {
            uiController.GridFill();
        }

        if (Input.GetKeyUp(KEY_NOISEPATTERN))
        {
            uiController.GridRandomize();
        }

        if (Input.GetKeyUp(KEY_RANDOMENTITY))
        {
            RandomizeEntityPositions();
        }

    }


    public void ToggleSimulation()
    {
        if (simulationActive)
        {
            star.transform.parent = null;
            star.SetPosition(star.GetHomePosition(), false);
            starchaser.SetPosition(starchaser.GetHomePosition(), false);
            starchaser.Pause();

            grid.ClearDebug();
            simulationActive = false;
            uiController.ToggleCover();
        }
        else
        {
            if (!spaceship.gameObject.activeSelf)
                return;
            if (!tradingPost.gameObject.activeSelf)
                return;
            if (!star.gameObject.activeSelf)
                return;
            if (!starchaser.gameObject.activeSelf)
                return;

            starchaser.Resume();
            simulationActive = true;
            uiController.ToggleCover();
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


    public void SetCameraSize(int size)
    {
        if (grid.SetGridSize(size))
            cam.orthographicSize = size;
    }

    public void SetSimulationSpeed(float speed)
    {
        starchaser.SetSimulationSpeed(speed);
    }

    public void RandomizeEntityPositions()
    {
        grid.RandomizeEntityPosition(spaceship, true);
        grid.RandomizeEntityPosition(tradingPost, true);
        grid.RandomizeEntityPosition(star, true);
        grid.RandomizeEntityPosition(starchaser, true);
        SelectEntity(null);
    }
}
