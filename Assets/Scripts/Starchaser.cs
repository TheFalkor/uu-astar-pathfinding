using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StarchaserState
{
    LOCATE_STAR,
    LOCATE_TRADINGPOST,
    LOCATE_SPACESHIP,
    GOTO_STAR,
    GOTO_TRADINGPOST,
    GOTO_SPACESHIP,
    STUCK
}

public class Starchaser : MonoBehaviour
{
    private AStarPath algorithm = new AStarPath();
    private StarchaserState state;
    private const int MAX_STAMINA = 10;
    private int currentStamina;

    void Start()
    {
        currentStamina = MAX_STAMINA;
        state = StarchaserState.LOCATE_STAR;
    }

    
    void Update()
    {
        
    }

    public void Test(Node start, Node end)
    {
        algorithm.CalculatePath(start, end);
    }
}
