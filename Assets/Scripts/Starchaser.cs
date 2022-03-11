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
        print(path.Count);
        for (int i = path.Count - 1; i >= 0; i--)
        {
            if (i > 0 && i < path.Count - 1)
            {
                path[i].DebugColor(path[i - 1].GetPosition(), path[i + 1].GetPosition(), true);
            }
        }
    }
}
