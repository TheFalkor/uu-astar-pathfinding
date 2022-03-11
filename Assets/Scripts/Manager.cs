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

        grid.RandomizeEntityPosition(spaceship);
        grid.RandomizeEntityPosition(tradingPost);
        grid.RandomizeEntityPosition(star);
        grid.RandomizeEntityPosition(starchaser);
    }


    void Update()
    {
        if(Input.GetKeyUp(KEY_STARTSIMULATION))
        {
            simulationActive = !simulationActive;
            if (simulationActive)
                starchaser.Resume();
            else
            {
                star.transform.parent = null;
                star.ResetPosition();
                starchaser.ResetPosition();

                grid.ClearDebug();
            }
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
            grid.RandomizeEntityPosition(star);
            grid.RandomizeEntityPosition(starchaser);
            SelectEntity(null);
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
