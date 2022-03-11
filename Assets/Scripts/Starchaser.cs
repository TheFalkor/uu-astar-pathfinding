using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StarchaserState
{
    LOCATE_TARGET,
    GOTO_TARGET,
    TRADE,
    REST,
    STUCK
}

public class Starchaser : MonoBehaviour
{
    private readonly AStarPath algorithm = new AStarPath();
    private StarchaserState state;
    private const int MAX_STAMINA = 15;
    private int currentStamina;

    void Start()
    {
        currentStamina = MAX_STAMINA;
        state = StarchaserState.LOCATE_TARGET;
    }

    
    void Update()
    {
        
    }

    public void Test(Node start, Node end)
    {
        algorithm.CalculatePath(start, end);
    }
}
