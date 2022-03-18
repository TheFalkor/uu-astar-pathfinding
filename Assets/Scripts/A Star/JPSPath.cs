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
        openNodes.Clear();
        closedNodes.Clear();
        path.Clear();

        Manager.instance.grid.ClearDebug();
        Vector2Int targetPosition = targetNode.GetPosition();

        openNodes.Add(new AStarNode(startNode));


        bool targetFound = false;
        while (openNodes.Count > 0 && !targetFound)
        {
            int nodeIndex = 0;
            for (int i = 0; i < openNodes.Count; i++)
            {
                if (openNodes[i].f < openNodes[nodeIndex].f)
                    nodeIndex = i;
            }

            AStarNode currentNode = openNodes[nodeIndex];

            openNodes.Remove(currentNode);
            List<Node> neighbourList = currentNode.node.GetNeighbours();

            for (int i = 0; i < neighbourList.Count; i++)
            {
                Vector2Int currentPosition = currentNode.node.GetPosition();
                int dX = currentPosition.x - neighbourList[i].GetPosition().x;
                int dY = currentPosition.y - neighbourList[i].GetPosition().y;

                Node jumpedNode = Jump(currentNode.node, dX, dY, targetNode);

                // jumpedNode != null
                    // if jump == target
                    // else
                        // caltulate fgh
                        // check if in open
                        // check if in closed
                        // no? then add to open
                // add current to closed
            }


        }


            return null;
    }


    private Node Jump(Node currentNode, int dX, int dY, Node targetNode)
    {
        return null;
    }
}
