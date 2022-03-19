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

            if (currentNode.node == targetNode)
            {
                Debug.Log("VICOTORY FOUND");
                return null;    // flip  path, return
            }

            IdentifySuccessor(currentNode, targetNode);

            closedNodes.Add(currentNode);
        }

        path.Reverse();

        return path;
    }


    private void IdentifySuccessor(AStarNode current, Node targetNode)
    {
        List<Node> neighbourList = GetNeighbour(current);

        for (int i = 0; i < neighbourList.Count; i++)
        {
            Vector2Int currentPos = current.node.GetPosition();
            Vector2Int nextPos = neighbourList[i].GetPosition();

            int dx = Mathf.Clamp(nextPos.x - currentPos.x, -1, 1);
            int dy = Mathf.Clamp(nextPos.y - currentPos.y, -1, 1);


            Node t = Jump(current.node, dx, dy, targetNode);
        }


    }

    private Node Jump(Node currentNode, int dX, int dY, Node targetNode)
    {
        Vector2Int nextPosition = currentNode.GetPosition();
        nextPosition.x += dX;
        nextPosition.y += dY;

        Node nextNode = Manager.instance.grid.GetNode(nextPosition);

        if (!nextNode.IsWalkable())
            return null;

        if (nextNode == targetNode)
            return nextNode;


        if(dX != 0 && dY != 0)  // Diagonal
        {

        }
        else                    // Horizontal & Vertical
        {
            if (dX != 0)
            {

            }
            else
            {

            }
        }



        return null;
    }

    private List<Node> GetNeighbour(AStarNode current)
    {
        List<Node> neighbourList = new List<Node>();

        if (current.parentNode != null)
        {
            Vector2Int currentPos = current.node.GetPosition();
            Vector2Int parentPos = current.parentNode.node.GetPosition();

            int dx = Mathf.Clamp(parentPos.x - currentPos.x, -1, 1);
            int dy = Mathf.Clamp(parentPos.y - currentPos.y, -1, 1);
            Node neighbour;

            // Diagonal
            if (dx != 0 && dy != 0)
            {
                neighbour = Manager.instance.grid.GetNode(new Vector2Int(currentPos.x + dx, currentPos.y + dy));
                if (neighbour != null && !neighbour.IsWalkable())
                    neighbourList.Add(neighbour);

                neighbour = Manager.instance.grid.GetNode(new Vector2Int(currentPos.x, currentPos.y + dy));
                if (neighbour != null && !neighbour.IsWalkable())
                    neighbourList.Add(neighbour);

                neighbour = Manager.instance.grid.GetNode(new Vector2Int(currentPos.x + dx, currentPos.y));
                if (neighbour != null && !neighbour.IsWalkable())
                    neighbourList.Add(neighbour);
            }
            // Horizontal / Vertical
            else
            {
                neighbour = Manager.instance.grid.GetNode(new Vector2Int(currentPos.x + dx, currentPos.y + dy));
                if (neighbour != null && !neighbour.IsWalkable())
                    neighbourList.Add(neighbour);

                neighbour = Manager.instance.grid.GetNode(new Vector2Int(currentPos.x + dx + dy, currentPos.y + dy + dx));
                if (neighbour != null && !neighbour.IsWalkable())
                    neighbourList.Add(neighbour);

                neighbour = Manager.instance.grid.GetNode(new Vector2Int(currentPos.x + dx - dy, currentPos.y + dy - dx));
                if (neighbour != null && !neighbour.IsWalkable())
                    neighbourList.Add(neighbour);
            }
        }
        else
        {
            List<Node> temp = current.node.GetNeighbours();

            for (int i = 0; i < temp.Count; i++)
                if (temp[i].IsWalkable())
                    neighbourList.Add(temp[i]);
        }


        return neighbourList;
    }

    private int CalculateG(Vector2Int current, Vector2Int newPos, int g)
    {
        if(current == newPos)
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
