using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StarchaserState
{
    LOCATE_TARGET,
    GOTO_TARGET,
    PREPARE,
    TRADE,
    REST,
    STUCK
}

public class Starchaser : Entity
{
    [Header("Settings")]
    private const int MAX_STAMINA = 10;
    private const float MOVEMENT_SPEED = 5.0f;
    private const float WAIT_TIME = 0.5f;
    private float simulationSpeed = 1.0f;


    [Header("Starchaser Tools")]
    private readonly AStarPath algorithm = new AStarPath();
    private List<Node> path;
    private StarchaserState state = StarchaserState.PREPARE;
    private Node target;


    [Header("Object References")]
    private Entity spaceship;
    private Entity tradingPost;
    private Entity star;


    [Header("Starchaser Variables")]
    private int currentStamina = MAX_STAMINA;
    private bool haveStar = false;
    private int curPathIndex = 0;
    private float currentTime = 0;
    private Vector2 currentTargetPosition;
    public int money = 0;
    private bool allowDrawPath = false;


    public void SetReferences(Entity spaceship, Entity tradingPost, Entity star)
    {
        this.spaceship = spaceship;
        this.tradingPost = tradingPost;
        this.star = star;
    }

    public void SetSimulationSpeed(float speed)
    {
        simulationSpeed = speed;
    }


    public void Resume()
    {
        currentStamina = MAX_STAMINA;
        money = 0;
        allowDrawPath = true;
        haveStar = false;
        currentTime = 0;

        SetTarget(star);
    }

    public void Pause()
    {
        allowDrawPath = false;
    }

    public void UpdateSimulation(float deltaTime)
    {
        switch (state)
        {
            case StarchaserState.LOCATE_TARGET:
                if(currentTime == 0)
                    FindTarget(Manager.instance.grid.GetNode(GetPosition()), target);

                currentTime += deltaTime * simulationSpeed;
                if (currentTime >= WAIT_TIME)
                {                 
                    if (path.Count == 0)
                        state = StarchaserState.STUCK;
                    else
                    {
                        curPathIndex = 1;
                        currentTargetPosition = path[curPathIndex].GetPosition();
                        state = StarchaserState.GOTO_TARGET;
                    }

                    currentTime = 0;
                }
                break;

            case StarchaserState.GOTO_TARGET:
                if (haveStar && currentStamina == 0)
                {
                    DropStar();
                    SetTarget(spaceship);
                    break;
                }
                
                if (transform.position == (Vector3)currentTargetPosition)
                {
                    if(curPathIndex == path.Count - 1)
                    {
                        state = StarchaserState.PREPARE;
                    }
                    else
                    {
                        if(haveStar)
                            currentStamina--;

                        if (currentStamina > 0 || !haveStar)
                        {   
                            curPathIndex++;
                            currentTargetPosition = path[curPathIndex].GetPosition();
                        }
                    }
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, currentTargetPosition, deltaTime * simulationSpeed * MOVEMENT_SPEED);
                }
                break;

            case StarchaserState.PREPARE:
                if(currentTargetPosition == spaceship.GetPosition() && currentStamina < MAX_STAMINA)
                {
                    state = StarchaserState.REST;
                }
                else if(currentTargetPosition == tradingPost.GetPosition() && haveStar)
                {
                    state = StarchaserState.TRADE;
                }
                else if (currentTargetPosition == star.GetPosition())
                {
                    PickupStar();
                    SetTarget(tradingPost);
            }
                else
                {
                    SetTarget(star);
                }
                break;

            case StarchaserState.TRADE:
                money++;
                Debug.Log("MONEYS: " + money);
                DropStar();
                Manager.instance.grid.RandomizeEntityPosition(star, false);
                state = StarchaserState.PREPARE;
                break;

            case StarchaserState.REST:
                currentStamina = MAX_STAMINA;
                state = StarchaserState.PREPARE;
                break;

            case StarchaserState.STUCK:
                Debug.Log("STARCHASER CANNOT FIND THE PATH!");
                break;
        }
    }


    private void SetTarget(Entity entity)
    {
        UpdatePosition();
        target = Manager.instance.grid.GetNode(entity.GetPosition());
        state = StarchaserState.LOCATE_TARGET;
    }

    private void FindTarget(Node start, Node end)
    {
        path = algorithm.CalculatePath(start, end);
        
        StartCoroutine(DrawPath());
    }


    private void PickupStar()
    {
        haveStar = true;
        star.transform.parent = this.transform;
    }

    private void DropStar()
    {
        haveStar = false;
        star.transform.parent = null;
        star.UpdatePosition();
    }

    private IEnumerator DrawPath()
    {
        int stamina = currentStamina + 1;

        for (int i = 0; i < path.Count; i++)
        {
            if (!allowDrawPath)
                break;

            if (i > 0 && i < path.Count - 1)
            {
                bool enoughStamina = stamina > 0;

                path[i].DrawPath(path[i - 1].GetPosition(), path[i + 1].GetPosition(), enoughStamina || !haveStar);
                yield return new WaitForSeconds(0.05f / simulationSpeed);
            }
            stamina--;
        }
    }
}
