using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPath
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
                if (neighbourList[i] == targetNode)
                {
                    path.Add(neighbourList[i]);
                    targetFound = true;
                    while (currentNode != null)
                    {
                        path.Add(currentNode.node);
                        currentNode = currentNode.parentNode;
                    }
                    break;
                }
                else
                {
                    if (neighbourList[i].blocked || !neighbourList[i].visible)
                        continue;

                    Vector2Int currentPos = currentNode.node.GetPosition();
                    Vector2Int neighbourPosition = neighbourList[i].GetPosition();

                    int scuffed = Mathf.Abs(currentPos.x - neighbourPosition.x) + Mathf.Abs(currentPos.y - neighbourPosition.y);
                    int g = currentNode.g;
                    g += scuffed == 1 ? 10 : 14;

                    int h = Mathf.Abs(targetPosition.x - neighbourPosition.x) + Mathf.Abs(targetPosition.y - neighbourPosition.y);
                    h *= 10;

                    int f = g + h;

                    bool handled = false;
                    for (int j = 0; j < openNodes.Count; j++)
                    {
                        if (openNodes[j].node.GetPosition() == neighbourList[i].GetPosition())
                        {
                            if (f < openNodes[j].f)
                            {
                                openNodes[j].f = f;
                                openNodes[j].g = g;
                                openNodes[j].h = h;
                                openNodes[j].parentNode = currentNode;
                            }
                            handled = true;
                            break;
                        }
                    }

                    if (!handled)
                    {
                        for (int j = 0; j < closedNodes.Count; j++)
                        {
                            if (closedNodes[j].node.GetPosition() == neighbourList[i].GetPosition())
                            {
                                //handled = true;
                                if(f >= closedNodes[j].f)
                                {
                                    handled = true; // This makes it very slow in some cases because it repeats same node multiple times.
                                }                   // Is it even necessary?
                                break;
                            }
                        }
                    }

                    if (!handled)
                    {
                        openNodes.Add(new AStarNode(neighbourList[i], currentNode, h, g, f));
                    }
                }
            }

            if(!targetFound)
                closedNodes.Add(currentNode);
        }

        path.Reverse();

        return path;
    }
}
