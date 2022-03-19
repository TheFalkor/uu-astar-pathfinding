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
            Debug.Log("WHILE LOOP");
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
                return path;    // flip  path, return
            }

            if(IdentifySuccessor(currentNode, targetNode))
            {
                Debug.Log("VICOTORY FOUND @");
                return path;
            }
            Debug.Log(openNodes.Count);
            closedNodes.Add(currentNode);
        }
        
        path.Reverse();

        return path;
    }


    private bool IdentifySuccessor(AStarNode current, Node targetNode)
    {
        List<Node> neighbourList = GetNeighbour(current);
        Debug.Log("NEIG: " + neighbourList.Count);
        for (int i = 0; i < neighbourList.Count; i++)
        {
            Vector2Int currentPos = current.node.GetPosition();
            Vector2Int nextPos = neighbourList[i].GetPosition();

            int dx = Mathf.Clamp(nextPos.x - currentPos.x, -1, 1);
            int dy = Mathf.Clamp(nextPos.y - currentPos.y, -1, 1);


            Node t = Jump(current.node, dx, dy, targetNode);

            if (t != null)
            {
                if(t == targetNode)
                {
                    return true;
                }
                Debug.Log("test? " + t.GetPosition());

                int g = CalculateG(currentPos, t.GetPosition(), current.g);
                int h = Mathf.Abs(targetNode.GetPosition().x - t.GetPosition().x) + Mathf.Abs(targetNode.GetPosition().y - t.GetPosition().y);
                h *= 10;

                int f = g + h;

                bool handled = false;
                for (int j = 0; j < openNodes.Count; j++)
                {
                    if (openNodes[j].node.GetPosition() == t.GetPosition())
                    {
                        if (f < openNodes[j].f)
                        {
                            openNodes[j].f = f;
                            openNodes[j].g = g;
                            openNodes[j].h = h;
                            openNodes[j].parentNode = current;
                        }
                        handled = true;
                        break;
                    }
                }

                if (!handled)
                {
                    for (int j = 0; j < closedNodes.Count; j++)
                    {
                        if (closedNodes[j].node.GetPosition() == t.GetPosition())
                        {
                            //handled = true;
                            if (f >= closedNodes[j].f)
                            {
                                handled = true; // This makes it very slow in some cases because it repeats same node multiple times.
                            }                   // Is it even necessary?
                            break;
                        }
                    }
                }

                if (!handled)
                {
                    Debug.Log("ADDED NEW NODE");
                    openNodes.Add(new AStarNode(t, current, h, g, f));
                }
            }
            // add to open? idk
        }

        return false;
    }

    private Node Jump(Node currentNode, int dX, int dY, Node targetNode)
    {
        Vector2Int nextPosition = currentNode.GetPosition();
        nextPosition.x += dX;
        nextPosition.y += dY;

        Node nextNode = Manager.instance.grid.GetNode(nextPosition);
        
        if (nextNode == null || !nextNode.IsWalkable())
            return null;

        Debug.Log("STAR: " + targetNode.GetPosition() + " :: CURRENT: " + nextNode.GetPosition());
        if (nextNode == targetNode)
        {
            Debug.Log("FOUND HOME");
            return nextNode;
        }


        if(dX != 0 && dY != 0)  // Diagonal
        {
            // diagnoadlas check


            Debug.Log("diag");
            if (Jump(nextNode, dX, 0, targetNode) != null || Jump(nextNode, 0, dY, targetNode) != null)
            {
                Debug.Log("something");
                return nextNode;
            }
        }
        else                    // Horizontal & Vertical
        {
            Debug.Log("straight" + nextNode.GetPosition());
            if (dX != 0)
            {
                Debug.Log("hori");
                if (HasForcedNeighbour(nextNode, new Vector2Int(dX, dY), new Vector2Int(0, 1)))
                {
                    return nextNode;
                }
                if (HasForcedNeighbour(nextNode, new Vector2Int(dX, dY), new Vector2Int(0, -1)))
                {
                    return nextNode;
                }
            }
            else
            {
                Debug.Log("vert");
                if (HasForcedNeighbour(nextNode, new Vector2Int(dX, dY), new Vector2Int(1, 0)))
                {
                    Debug.Log("right");
                    return nextNode;
                }
                if (HasForcedNeighbour(nextNode, new Vector2Int(dX, dY), new Vector2Int(-1, 0)))
                {
                    Debug.Log("left");
                    return nextNode;
                }
            }
        }



        return Jump(nextNode, dX, dY, targetNode);
    }

    private List<Node> GetNeighbour(AStarNode current)
    {
        List<Node> neighbourList = new List<Node>();
        if (current.parentNode != null)
        {
            Vector2Int currentPos = current.node.GetPosition();
            Vector2Int parentPos = current.parentNode.node.GetPosition();

            int dx = Mathf.Clamp(parentPos.x - currentPos.x, -1, 1) * -1;
            int dy = Mathf.Clamp(parentPos.y - currentPos.y, -1, 1) * -1;
            Node neighbour;
            Debug.Log("CHECKNEI: " + dx + ", " + dy);

            // Diagonal
            if (dx != 0 && dy != 0)
            {
                neighbour = Manager.instance.grid.GetNode(new Vector2Int(currentPos.x + dx, currentPos.y + dy));
                if (neighbour != null && neighbour.IsWalkable())
                    neighbourList.Add(neighbour);

                neighbour = Manager.instance.grid.GetNode(new Vector2Int(currentPos.x, currentPos.y + dy));
                if (neighbour != null && neighbour.IsWalkable())
                    neighbourList.Add(neighbour);

                neighbour = Manager.instance.grid.GetNode(new Vector2Int(currentPos.x + dx, currentPos.y));
                if (neighbour != null && neighbour.IsWalkable())
                    neighbourList.Add(neighbour);
            }
            // Horizontal / Vertical
            else
            {
                neighbour = Manager.instance.grid.GetNode(new Vector2Int(currentPos.x + dx, currentPos.y + dy));
                if (neighbour != null && neighbour.IsWalkable())
                {
                    Debug.Log("ADDED A NEIGHBOUR1");
                    neighbourList.Add(neighbour);
                }

                neighbour = Manager.instance.grid.GetNode(new Vector2Int(currentPos.x + dx + dy, currentPos.y + dy + dx));
                if (neighbour != null && neighbour.IsWalkable())
                {
                    Debug.Log("ADDED A NEIGHBOUR2");
                    neighbourList.Add(neighbour);
                }
                else
                {
                    //Debug.Log("CANT ADD: " + neighbour.GetPosition());
                }

                neighbour = Manager.instance.grid.GetNode(new Vector2Int(currentPos.x + dx - dy, currentPos.y + dy - dx));
                if (neighbour != null && neighbour.IsWalkable())
                {
                    Debug.Log("ADDED A NEIGHBOUR3");
                    neighbourList.Add(neighbour);
                }
                else
                {
                    //Debug.Log("CANT ADD: " + neighbour.GetPosition());
                }
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


    private bool HasForcedNeighbour(Node current, Vector2Int direction, Vector2Int offset)
    {
        Node offsetNode = Manager.instance.grid.GetNode(current.GetPosition() + offset);
        if (offsetNode != null && !offsetNode.IsWalkable())
        {
            Vector2Int nextPos = current.GetPosition() + direction;
            Node next = Manager.instance.grid.GetNode(nextPos);

            if (next != null && next.IsWalkable())
            {
                Vector2Int neighbourPos = current.GetPosition() + direction + offset;
                Node neighbour = Manager.instance.grid.GetNode(neighbourPos);

                if (neighbour != null && neighbour.IsWalkable())
                    return true;
               // return neighbour;
            }
        }

        return false;
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
