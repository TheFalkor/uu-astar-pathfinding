using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{
    [Header("AStarNode Variables")]
    public int h = 0;   // Estimated cost from this node to target
    public int g = 0;   // Total cost to get to this node from start
    public int f = 0;   // Sum of H and G
    public Node node;
    public AStarNode parentNode;

    public AStarNode(Node node, AStarNode parent = null, int h = 0, int g = 0, int f = 0)
    {
        this.node = node;
        parentNode = parent;

        this.h = h;
        this.g = g;
        this.f = f;
    }
}
