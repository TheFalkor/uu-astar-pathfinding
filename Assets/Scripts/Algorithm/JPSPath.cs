using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPSPath
{
    [Header("AStarPath Variables")]
    private List<AStarNode> openList = new List<AStarNode>();
    private List<AStarNode> closedList = new List<AStarNode>();
    private readonly List<Cell> path = new List<Cell>();

    GridController grid;
    private Cell target;


    public List<Cell> CalculatePath(Cell start, Cell target)
    {
        this.target = target;

        openList.Clear();
        closedList.Clear();
        path.Clear();
        if (grid == null)
            grid = Manager.instance.grid;

        grid.ClearDebug();

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
                return FillPath(path);
            }

            IdentifySuccessors(current);
        }

        return path;
    }


    private void IdentifySuccessors(AStarNode node)
    {
        List<Vector2Int> neighborPositions = FindNeighbors(node);

        for (int i = 0; i < neighborPositions.Count; i++)
        {
            Vector2Int currentPosition = node.cell.GetPosition();
            int dx = neighborPositions[i].x - currentPosition.x;
            int dy = neighborPositions[i].y - currentPosition.y;

            Cell jumpedCell = Jump(neighborPositions[i].x, neighborPositions[i].y, dx, dy);

            if(jumpedCell != null)
            {
                Vector2Int jumpPosition = jumpedCell.GetPosition();

                for (int j = 0; j < closedList.Count; j++)
                    if (closedList[j].cell == jumpedCell)
                        continue;


                int g = CalculateG(node.cell.GetPosition(), jumpPosition, node.g);
                int h = CalculateH(jumpPosition, target.GetPosition());

                for (int j = 0; j < openList.Count; j++)
                {
                    if (openList[j].cell == jumpedCell)
                    {
                        if (g < openList[j].g)
                        {
                            openList[j].g = g;
                            openList[j].h = h;
                            openList[j].f = g + h;
                            openList[j].parentNode = node;
                        }

                        continue;
                    }
                }

                openList.Add(new AStarNode(jumpedCell, node, h, g, g + h));
            }
        }
    }
    

    private Cell Jump(int x, int y, int dx, int dy)
    {
        if (!grid.WalkableAt(x, y))
            return null;

        Cell current = grid.GetCellUsingGrid(x, y);

        if (current == target)
            return current;


        if (dx != 0 && dy != 0)
        {
            if ((grid.WalkableAt(x - dx, y + dy) && !grid.WalkableAt(x - dx, y)) ||
                grid.WalkableAt(x + dx, y - dy) && !grid.WalkableAt(x, y - dy))
                return current;

            if (Jump(x + dx, y, dx, 0) || Jump(x, y + dy, 0, dy))
                return current;
        }
        else
        {
            if (dx != 0)
            {
                if (grid.WalkableAt(x + dx, y + 1) && !grid.WalkableAt(x, y + 1) ||
                    grid.WalkableAt(x + dx, y - 1) && !grid.WalkableAt(x, y - 1))
                    return current;
            }
            else
            {
                if (grid.WalkableAt(x + 1, y + dy) && !grid.WalkableAt(x + 1, y) ||
                    grid.WalkableAt(x - 1, y + dy) && !grid.WalkableAt(x - 1, y))
                    return current;
            }
        }


        return Jump(x + dx, y + dy, dx, dy);
    }
    

    private List<Vector2Int> FindNeighbors(AStarNode node)
    {
        List<Vector2Int> list = new List<Vector2Int>();

        if (node.parentNode != null)
        {
            Vector2Int currentPosition = node.cell.GetPosition();
            Vector2Int deltaPosition = currentPosition - node.parentNode.cell.GetPosition();
            int x = currentPosition.x;
            int y = currentPosition.y;
            int dx = Mathf.Clamp(deltaPosition.x, -1, 1);
            int dy = Mathf.Clamp(deltaPosition.y, -1, 1);

            if (dx != 0 && dy != 0)
            {
                if (grid.WalkableAt(x, y + dy))
                    list.Add(new Vector2Int(x, y + dy));

                if (grid.WalkableAt(x + dx, y))
                    list.Add(new Vector2Int(x + dx, y));

                if (grid.WalkableAt(x + dx, y + dy))
                    list.Add(new Vector2Int(x + dx, y + dy));

                if (!grid.WalkableAt(x - dx, y))
                    list.Add(new Vector2Int(x - dx, y + dy));

                if (!grid.WalkableAt(x, y - dy))
                    list.Add(new Vector2Int(x + dx, y - dy));
            }
            else
            {
                int xf = dx * dx;
                int yf = dy * dy;
                if (grid.WalkableAt(x + dx, y + dy))
                    list.Add(new Vector2Int(x + dx, y + dy));

                if (!grid.WalkableAt(x + yf, y + xf))
                    list.Add(new Vector2Int(x + dx + yf, y + dy + xf));

                if (!grid.WalkableAt(x - yf, y - xf))
                    list.Add(new Vector2Int(x + dx - yf, y + dy - xf));
            }
        }
        else
        {
            List<Cell> cellNeighbors = node.cell.GetNeighbors();
            for (int i = 0; i < cellNeighbors.Count; i++)
                list.Add(cellNeighbors[i].GetPosition());
        }

        return list;
    }


    private List<Cell> FillPath(List<Cell> path)
    {
        if (path.Count == 0)
            return path;

        List<Cell> newPath = new List<Cell>();

        newPath.Add(path[0]);

        for (int i = 0; i < path.Count - 1; i++)
        {
            if (i != 0 && i != path.Count - 1)
                path[i].SetNodeMarker();

            int dx = Mathf.Clamp(path[i + 1].GetPosition().x - path[i].GetPosition().x, -1, 1);
            int dy = Mathf.Clamp(path[i + 1].GetPosition().y - path[i].GetPosition().y, -1, 1);

            while (newPath[newPath.Count - 1] != path[i + 1])
            {
                Vector2Int position = newPath[newPath.Count - 1].GetPosition();
                newPath.Add(grid.GetCellUsingGrid(position.x + dx, position.y + dy));
            }
        }

        return newPath;
    }


    private int CalculateG(Vector2Int current, Vector2Int goal, int g)
    {
        if (current == goal)
            return g;

        int dx = Mathf.Clamp(goal.x - current.x, -1, 1);
        int dy = Mathf.Clamp(goal.y - current.y, -1, 1);

        current.x += dx;
        current.y += dy;

        return CalculateG(current, goal, g + dx != 0 && dy != 0 ? 14 : 10);
    }

    private int CalculateH(Vector2Int current, Vector2Int target)
    {
        return (Mathf.Abs(target.x - current.x) + Mathf.Abs(target.y - current.y)) * 10;
    }
}
