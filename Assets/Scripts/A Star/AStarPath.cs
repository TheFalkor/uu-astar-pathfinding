using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPath
{
    [Header("AStarPath Variables")]
    private List<AStarNode> openList = new List<AStarNode>();
    private List<AStarNode> closedList = new List<AStarNode>();
    private readonly List<Cell> path = new List<Cell>();


    public List<Cell> CalculatePath(Cell start, Cell target)
    {
        openList.Clear();
        closedList.Clear();
        path.Clear();

        Manager.instance.grid.ClearDebug();

        Vector2Int targetPosition = target.GetPosition();

        openList.Add(new AStarNode(start));


        while (openList.Count > 0)
        {
            int nodeIndex = 0;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].f < openList[nodeIndex].f)
                    nodeIndex = i;
            }

            AStarNode currentNode = openList[nodeIndex];

            openList.Remove(currentNode);
            List<Cell> neighborList = currentNode.cell.GetNeighbors();

            for (int i = 0; i < neighborList.Count; i++)
            {
                if (neighborList[i] == target)
                {
                    path.Add(neighborList[i]);
                    while (currentNode != null)
                    {
                        path.Add(currentNode.cell);
                        currentNode = currentNode.parentNode;
                    }

                    path.Reverse();
                    return path;
                }
                else
                {
                    if (neighborList[i].blocked || !neighborList[i].visible)
                        continue;

                    Vector2Int currentPos = currentNode.cell.GetPosition();
                    Vector2Int neighborPosition = neighborList[i].GetPosition();

                    int scuffed = Mathf.Abs(currentPos.x - neighborPosition.x) + Mathf.Abs(currentPos.y - neighborPosition.y);
                    int g = currentNode.g;
                    g += scuffed == 1 ? 10 : 14;

                    int h = Mathf.Abs(targetPosition.x - neighborPosition.x) + Mathf.Abs(targetPosition.y - neighborPosition.y);
                    h *= 10;

                    int f = g + h;

                    bool handled = false;
                    for (int j = 0; j < openList.Count; j++)
                    {
                        if (openList[j].cell.GetPosition() == neighborList[i].GetPosition())
                        {
                            if (f < openList[j].f)
                            {
                                openList[j].f = f;
                                openList[j].g = g;
                                openList[j].h = h;
                                openList[j].parentNode = currentNode;
                            }
                            handled = true;
                            break;
                        }
                    }

                    if (!handled)
                    {
                        for (int j = 0; j < closedList.Count; j++)
                        {
                            if (closedList[j].cell.GetPosition() == neighborList[i].GetPosition())
                            {
                                if(f >= closedList[j].f)
                                {
                                    handled = true;
                                }
                                break;
                            }
                        }
                    }

                    if (!handled)
                    {
                        openList.Add(new AStarNode(neighborList[i], currentNode, h, g, f));
                    }
                }
            }

            closedList.Add(currentNode);
        }


        return path;
    }
}
