using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPSPathRewritten
{
    [Header("AStarPath Variables")]
    private List<AStarNode> openNodes = new List<AStarNode>();
    private List<AStarNode> closedNodes = new List<AStarNode>();
    private readonly List<Node> path = new List<Node>();

    private Node theTarget;

    public List<Node> CalculatePath(Node startNode, Node targetNode)
    {
        openNodes.Clear();
        closedNodes.Clear();
        path.Clear();

        Manager.instance.grid.ClearDebug();
        theTarget = targetNode;

        // SOURCE:
        // https://github.com/qiao/PathFinding.js/blob/master/src/finders/JumpPointFinderBase.js

        openNodes.Add(new AStarNode(startNode));


        while (openNodes.Count > 0)
        {
            // Pop smallest F
            int nodeIndex = 0;
            for (int i = 0; i < openNodes.Count; i++)
            {
                if (openNodes[i].f < openNodes[nodeIndex].f)
                    nodeIndex = i;
            }

            AStarNode currentNode = openNodes[nodeIndex];

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            if (currentNode.node == targetNode)
            {
                path.Add(targetNode);
                while (currentNode != null)
                {
                    path.Add(currentNode.node);
                    currentNode = currentNode.parentNode;
                }
                path.Reverse();
                return path;
            }

            IdentifySuccessors(currentNode);

        }

        return path;
    }


    private void IdentifySuccessors(AStarNode node)
    {
        List<AStarNode> neighbourList = FindNeighbours(node);

        for (int i = 0; i < neighbourList.Count; i++)
        {
            AStarNode neighbour = neighbourList[i];

            Node jumpPoint = Jump(neighbour.node.GetPosition().x, neighbour.node.GetPosition().y, node.node.GetPosition().x, node.node.GetPosition().y); 
        
            if(jumpPoint)
            {
                int jx = jumpPoint.GetPosition().x;
                int jy = jumpPoint.GetPosition().y;

                for (int j = 0; j < closedNodes.Count; j++)
                {
                    if (closedNodes[j].node == jumpPoint)
                        continue;
                }

                int g = CalculateG(node.node.GetPosition(), jumpPoint.GetPosition(), node.g);
                int h = (Mathf.Abs(theTarget.GetPosition().x - jx) + Mathf.Abs(theTarget.GetPosition().y - jy)) * 10;

                bool found = false;
                for (int j = 0; j < openNodes.Count; j++)
                {
                    if(openNodes[j].node == jumpPoint)
                    {
                        found = true;

                        if(g < openNodes[j].g)
                        {
                            openNodes[j].g = g;
                            openNodes[j].h = h;
                            openNodes[j].f = openNodes[j].g + openNodes[j].h;
                            openNodes[j].parentNode = node;
                        }
                    }
                }

                if (!found)
                {
                    AStarNode jumpPointA = new AStarNode(jumpPoint, node, h, g, g + h);
                    openNodes.Add(jumpPointA);
                }
            }
        }
    }


    private Node Jump(int x, int y, int px, int py)
    {
        int dx = x - px;
        int dy = y - py;

        if (!Manager.instance.grid.WalkableAt(x, y))
            return null;

        // JumpRecursionTrack???
        Node xyNode = Manager.instance.grid.GetNodeGrid(new Vector2Int(x, y));
        if (xyNode == theTarget)
            return xyNode;
        

        // Check Forced neighbour along diagnol
        if (dx != 0 && dy != 0)
        {
            if ((Manager.instance.grid.WalkableAt(x - dx, y + dy) && !Manager.instance.grid.WalkableAt(x - dx, y)) ||
                 Manager.instance.grid.WalkableAt(x + dx, y - dy) && !Manager.instance.grid.WalkableAt(x, y - dy))
                return xyNode;

            if (Jump(x + dx, y, x, y) || Jump(x, y + dy, x, y))
                return xyNode;
        }
        // Horizontally / Vertically
        else
        {
            if (dx != 0)    // Moving with X
            {
                if ((Manager.instance.grid.WalkableAt(x + dx, y + 1) && !Manager.instance.grid.WalkableAt(x, y + 1)) ||
                     Manager.instance.grid.WalkableAt(x + dx, y - 1) && !Manager.instance.grid.WalkableAt(x, y - 1))
                    return xyNode;
            }
            else            // Moving with Y
            {
                if ((Manager.instance.grid.WalkableAt(x + 1, y + dy) && !Manager.instance.grid.WalkableAt(x + 1, y)) ||
                     Manager.instance.grid.WalkableAt(x - 1, y + dy) && !Manager.instance.grid.WalkableAt(x - 1, y))
                    return xyNode;
            }
        }



        return Jump(x + dx, y + dy, x, y);
    }


    private List<AStarNode> FindNeighbours(AStarNode node)
    {
        AStarNode parent = node.parentNode;
        List<AStarNode> list = new List<AStarNode>();

        Vector2Int nodePos = node.node.GetPosition();
        int x = nodePos.x;
        int y = nodePos.y;

        if(parent != null)
        {
            Vector2Int parentPos = parent.node.GetPosition();
            int px = parentPos.x;
            int py = parentPos.y;

            int dx = (x - px) / Mathf.Max(Mathf.Abs(x - px), 1);
            int dy = (y - py) / Mathf.Max(Mathf.Abs(y - py), 1);


            if (dx != 0 && dy != 0)
            {
                if (Manager.instance.grid.WalkableAt(x, y + dy))
                    list.Add(new AStarNode(Manager.instance.grid.GetNodeGrid(new Vector2Int(x, y + dy)), node));

                if (Manager.instance.grid.WalkableAt(x + dx, y))
                    list.Add(new AStarNode(Manager.instance.grid.GetNodeGrid(new Vector2Int(x + dx, y)), node));

                if (Manager.instance.grid.WalkableAt(x + dx, y + dy))
                    list.Add(new AStarNode(Manager.instance.grid.GetNodeGrid(new Vector2Int(x + dx, y + dy)), node));

                if (!Manager.instance.grid.WalkableAt(x - dx, y))
                    list.Add(new AStarNode(Manager.instance.grid.GetNodeGrid(new Vector2Int(x - dx, y + dy)), node));

                if (!Manager.instance.grid.WalkableAt(x, y - dy))
                    list.Add(new AStarNode(Manager.instance.grid.GetNodeGrid(new Vector2Int(x + dx, y - dy)), node));
            }
            else
            {
                if (dx == 0)
                {
                    if (Manager.instance.grid.WalkableAt(x, y + dy))
                        list.Add(new AStarNode(Manager.instance.grid.GetNodeGrid(new Vector2Int(x, y + dy)), node));

                    if (!Manager.instance.grid.WalkableAt(x + 1, y))
                        list.Add(new AStarNode(Manager.instance.grid.GetNodeGrid(new Vector2Int(x + 1, y + dy)), node));

                    if (!Manager.instance.grid.WalkableAt(x - 1, y))
                        list.Add(new AStarNode(Manager.instance.grid.GetNodeGrid(new Vector2Int(x - 1, y + dy)), node));
                }
                else
                {
                    if (Manager.instance.grid.WalkableAt(x + dx, y))
                        list.Add(new AStarNode(Manager.instance.grid.GetNodeGrid(new Vector2Int(x + dx, y)), node));

                    if (!Manager.instance.grid.WalkableAt(x, y + 1))
                        list.Add(new AStarNode(Manager.instance.grid.GetNodeGrid(new Vector2Int(x + dx, y + 1)), node));

                    if (!Manager.instance.grid.WalkableAt(x, y - 1))
                        list.Add(new AStarNode(Manager.instance.grid.GetNodeGrid(new Vector2Int(x + dx, y - 1)), node));
                }
            }
        }
        else
        {
            List<Node> all = node.node.GetNeighbours();
            for (int i = 0; i < all.Count; i++)
            {
                list.Add(new AStarNode(all[i], node));
            }
        }

        return list;
    }

    private int CalculateG(Vector2Int current, Vector2Int newPos, int g)
    {
        if (current == newPos)
        {
            return g;
        }
        if (current.x == newPos.x || current.y == newPos.y)
        {
            current.x += Mathf.Clamp(newPos.x - current.x, -1, 1);
            current.y += Mathf.Clamp(newPos.y - current.y, -1, 1);

            g = CalculateG(current, newPos, g + 10);
        }
        else
        {
            current.x += Mathf.Clamp(newPos.x - current.x, -1, 1);
            current.y += Mathf.Clamp(newPos.y - current.y, -1, 1);

            g = CalculateG(current, newPos, g + 14);
        }

        return g;
    }
}
