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
    private bool haveStar = false;

    private List<Node> path;

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
        path = algorithm.CalculatePath(start, end);

        int stamina = currentStamina + 1;
        for (int i = 0; i < path.Count; i++)
        {
            if (i > 0 && i < path.Count - 1)
            {
                bool enoughStamina = stamina > 0;

                path[i].DrawPath(path[i - 1].GetPosition(), path[i + 1].GetPosition(), enoughStamina);
            }
            stamina--;
        }
    }
}
