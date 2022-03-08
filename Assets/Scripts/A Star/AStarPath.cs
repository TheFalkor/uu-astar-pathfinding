using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPath
{
    private List<AStarNode> openNodes = new List<AStarNode>();
    private List<AStarNode> closedNodes = new List<AStarNode>();
    public readonly List<Node> path = new List<Node>();

    public void CalculatePath(Node startNode, Node targetNode)
    {
        openNodes.Clear();
        closedNodes.Clear();
        path.Clear();

        Vector2Int targetPosition = targetNode.GetPosition();

        openNodes.Add(new AStarNode(startNode));

        while (openNodes.Count > 0)
        {
            Debug.Log("a");
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

                    while (currentNode != null)
                    {
                        path.Add(currentNode.node);
                        currentNode = currentNode.parentNode;
                    }
                    openNodes.Clear();
                    break;
                }
                else
                {
                    if (neighbourList[i].blocked || !neighbourList[i].visible)
                        continue;
                    neighbourList[i].DebugColor(2);

                    int g = currentNode.g + 10;

                    Vector2Int neighbourPosition = neighbourList[i].GetPosition();
                    int h = Mathf.Abs(targetPosition.x - neighbourPosition.x) + Mathf.Abs(targetPosition.y - neighbourPosition.y);

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
                                handled = true;
                                /*if(f >= closedNodes[j].f)
                                {
                                    handled = true; // This makes it very slow in some cases because it repeats same node multiple times.
                                }*/                 // Is it even necessary?
                                break;
                            }
                        }
                    }

                    if (!handled)
                    {
                        openNodes.Add(new AStarNode(neighbourList[i], currentNode, h, g, f));
                        currentNode.node.DebugColor(1);
                    }
                }
            }

            closedNodes.Add(currentNode);
        }

        for (int i = path.Count - 1; i >= 0; i--)
        {
            //Debug.Log(i + " : " + path[i].GetPosition());
            path[i].DebugColor(0);
        }
    }
}
