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

        bool targetFound = false;
        while (openList.Count > 0 && !targetFound)
        {
            int nodeIndex = 0;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].f < openList[nodeIndex].f)
                    nodeIndex = i;
            }

            AStarNode currentNode = openList[nodeIndex];

            openList.Remove(currentNode);
            List<Cell> neighbourList = currentNode.cell.GetNeighbours();

            for (int i = 0; i < neighbourList.Count; i++)
            {
                if (neighbourList[i] == target)
                {
                    path.Add(neighbourList[i]);
                    targetFound = true;
                    while (currentNode != null)
                    {
                        path.Add(currentNode.cell);
                        currentNode = currentNode.parentNode;
                    }
                    break;
                }
                else
                {
                    if (neighbourList[i].blocked || !neighbourList[i].visible)
                        continue;

                    Vector2Int currentPos = currentNode.cell.GetPosition();
                    Vector2Int neighbourPosition = neighbourList[i].GetPosition();

                    int scuffed = Mathf.Abs(currentPos.x - neighbourPosition.x) + Mathf.Abs(currentPos.y - neighbourPosition.y);
                    int g = currentNode.g;
                    g += scuffed == 1 ? 10 : 14;

                    int h = Mathf.Abs(targetPosition.x - neighbourPosition.x) + Mathf.Abs(targetPosition.y - neighbourPosition.y);
                    h *= 10;

                    int f = g + h;

                    bool handled = false;
                    for (int j = 0; j < openList.Count; j++)
                    {
                        if (openList[j].cell.GetPosition() == neighbourList[i].GetPosition())
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
                            if (closedList[j].cell.GetPosition() == neighbourList[i].GetPosition())
                            {
                                //handled = true;
                                if(f >= closedList[j].f)
                                {
                                    handled = true; // This makes it very slow in some cases because it repeats same cell multiple times.
                                }                   // Is it even necessary?
                                break;
                            }
                        }
                    }

                    if (!handled)
                    {
                        openList.Add(new AStarNode(neighbourList[i], currentNode, h, g, f));
                    }
                }
            }

            if(!targetFound)
                closedList.Add(currentNode);
        }

        path.Reverse();

        return path;
    }
}
