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

public enum Algorithm
{
    ASTAR,
    JPS
}

public class Starchaser : Entity
{
    [Header("Settings")]
    private int MAX_STAMINA = 10;
    private const float MOVEMENT_SPEED = 5.0f;
    private const float WAIT_TIME = 0.5f;
    private float simulationSpeed = 1.0f;


    [Header("Starchaser Tools")]
    private readonly AStarPath astarAlgorithm = new AStarPath();
    private readonly JPSPath jpsAlgorithm = new JPSPath();
    private List<Cell> path;
    private StarchaserState state = StarchaserState.PREPARE;
    private Cell target;
    private Algorithm currentAlgorithm = Algorithm.ASTAR;


    [Header("Object References")]
    private Entity spaceship;
    private Entity tradingPost;
    private Entity star;


    [Header("Starchaser Variables")]
    private int currentStamina;
    private bool haveStar = false;
    private bool wantStar = false;
    private int curPathIndex = 0;
    private float currentTime = 0;
    private Vector2 currentTargetPosition;
    private int starsSold = 0;
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

    public void SetStamina(int stamina)
    {
        MAX_STAMINA = stamina;
    }

    public void SetAlgorithm(Algorithm algorithm)
    {
        currentAlgorithm = algorithm;
    }


    public void Resume()
    {
        currentStamina = MAX_STAMINA;
        starsSold = 0;
        allowDrawPath = true;
        haveStar = false;
        wantStar = true;
        currentTime = 0;

        Manager.instance.UpdateStarUI(starsSold);
        Manager.instance.UpdateStaminaUI(currentStamina);
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
                    FindTarget(Manager.instance.grid.GetCellUsingReal(GetPosition()), target);

                currentTime += deltaTime * simulationSpeed;
                if (currentTime >= WAIT_TIME)
                {                 
                    if (path.Count == 0)
                        state = StarchaserState.STUCK;
                    else
                    {
                        curPathIndex = 1;
                        currentTargetPosition = path[curPathIndex].GetRealPosition();
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
                        if (haveStar)
                        {
                            currentStamina--;
                            Manager.instance.UpdateStaminaUI(currentStamina);
                        }

                        state = StarchaserState.PREPARE;
                    }
                    else
                    {
                        if(haveStar)
                        {
                            currentStamina--;
                            Manager.instance.UpdateStaminaUI(currentStamina);
                        }
                            
                        if (currentStamina > 0 || !haveStar)
                        {   
                            curPathIndex++;
                            currentTargetPosition = path[curPathIndex].GetRealPosition();
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
                else if (currentTargetPosition == star.GetPosition() && wantStar)
                {
                    PickupStar();
                    SetTarget(tradingPost);
            }
                else
                {
                    if (currentStamina > 0)
                    {
                        wantStar = true;
                        SetTarget(star);
                    }
                    else
                        SetTarget(spaceship);
                }
                break;

            case StarchaserState.TRADE:
                starsSold++;
                Manager.instance.UpdateStarUI(starsSold);

                DropStar();
                Manager.instance.grid.RandomizeEntityPosition(star, false);
                state = StarchaserState.PREPARE;
                break;

            case StarchaserState.REST:
                currentStamina = MAX_STAMINA;
                Manager.instance.UpdateStaminaUI(currentStamina);
                state = StarchaserState.PREPARE;
                break;

            case StarchaserState.STUCK:
                //Debug.Log("STARCHASER CANNOT FIND THE PATH!");
                break;
        }
    }


    private void SetTarget(Entity entity)
    {
        UpdatePosition();
        target = Manager.instance.grid.GetCellUsingReal(entity.GetPosition());
        state = StarchaserState.LOCATE_TARGET;
    }

    private void FindTarget(Cell start, Cell end)
    {
        if (currentAlgorithm == Algorithm.ASTAR)
            path = astarAlgorithm.CalculatePath(start, end);
        else
            path = jpsAlgorithm.CalculatePath(start, end);

        StartCoroutine(DrawPath());
    }


    private void PickupStar()
    {
        haveStar = true;
        star.transform.parent = this.transform;
    }

    private void DropStar()
    {
        wantStar = false;
        haveStar = false;
        star.transform.parent = null;
        star.UpdatePosition();
    }

    private IEnumerator DrawPath()
    {
        int stamina = currentStamina + 1;

        if (!haveStar)
            stamina = MAX_STAMINA;

        for (int i = 0; i < path.Count; i++)
        {
            if (!allowDrawPath)
                break;

            if (i > 0 && i < path.Count - 1)
            {
                path[i].DrawPath(path[i - 1].GetRealPosition(), path[i + 1].GetRealPosition(), stamina);
                yield return new WaitForSeconds(0.05f / simulationSpeed);
            }

            if (haveStar)
                stamina--;
        }
    }
}
