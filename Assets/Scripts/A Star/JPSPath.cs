using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPSPath
{
    [Header("AStarPath Variables")]
    private List<AStarNode> openNodes = new List<AStarNode>();
    private List<AStarNode> closedNodes = new List<AStarNode>();
    private readonly List<Node> path = new List<Node>();

    public List<Node> CalculatePath(Node startNode, Node targetNode)
    {
        return null;
    }
}
