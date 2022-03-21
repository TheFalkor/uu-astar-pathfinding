using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPSPath
{
    [Header("AStarPath Variables")]
    private List<AStarNode> openList = new List<AStarNode>();
    private List<AStarNode> closedList = new List<AStarNode>();
    private readonly List<Cell> path = new List<Cell>();

    private Cell target;


    public List<Cell> CalculatePath(Cell start, Cell target)
    {
        this.target = target;

        openList.Clear();
        closedList.Clear();
        path.Clear();

        Manager.instance.grid.ClearDebug();

        openList.Add(new AStarNode(start));


        while (openList.Count > 0)
        {
            int nodeIndex = 0;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].f < openList[nodeIndex].f)
                    nodeIndex = i;
            }

            AStarNode current = openList[nodeIndex];

            openList.Remove(current);
            closedList.Add(current);


            if (current.cell == target)
            {
                while (current != null)
                {
                    path.Add(current.cell);
                    current = current.parentNode;
                }

                path.Reverse();
                return path;
            }

            IdentifySuccessors(current);
        }

        return path;
    }


    private void IdentifySuccessors(AStarNode node)
    {

    }

}
