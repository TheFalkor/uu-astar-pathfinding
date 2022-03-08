using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    [Header("Grid Settings")]
    private const int MIN_CAMERA_SIZE = 5;
    private const int MAX_CAMERA_SIZE = 15;


    [Header("Grid Variables")]
    private const int ROW_COUNT = MAX_CAMERA_SIZE * 2;
    private int currentCameraSize;
    private readonly Node[] nodeArray = new Node[ROW_COUNT * ROW_COUNT];
    private readonly List<Node> visibleNodeList = new List<Node>();


    public Grid(GameObject nodePrefab, int startSize)
    {
        currentCameraSize = startSize;
        Transform nodeParent = new GameObject("Nodes").transform;

        int minVisibleIndex = (MAX_CAMERA_SIZE - startSize) * ROW_COUNT;
        int maxVisibleIndex = nodeArray.Length - minVisibleIndex - 1;
        for (int i = 0; i < nodeArray.Length; i++)
        {
            Node node = Object.Instantiate(nodePrefab, new Vector2(-MAX_CAMERA_SIZE + i % ROW_COUNT, MAX_CAMERA_SIZE - i / ROW_COUNT), Quaternion.identity, nodeParent).GetComponent<Node>();
            if (i > minVisibleIndex && i < maxVisibleIndex)
            {
                if (i % ROW_COUNT >= MAX_CAMERA_SIZE - startSize && i % ROW_COUNT < ROW_COUNT - MAX_CAMERA_SIZE + startSize)
                {
                    node.SetNodeVisible(true);
                    visibleNodeList.Add(node);
                }
                else
                    node.SetNodeVisible(false);
            }
            else
                node.SetNodeVisible(false);

            nodeArray[i] = node;
        }
    }


    public void SetNodeBlocked(Vector2Int position, bool blocked)
    {
        if (position.x < -currentCameraSize || position.x >= currentCameraSize || position.y <= -currentCameraSize || position.y > currentCameraSize)
            return;

        int index = (position.x + MAX_CAMERA_SIZE) % ROW_COUNT + (MAX_CAMERA_SIZE - position.y) * ROW_COUNT;
        nodeArray[index].SetNodeBlocked(blocked);
    }


    public bool ChangeGridSize(int direction)
    {
        if (currentCameraSize + direction > MAX_CAMERA_SIZE || currentCameraSize + direction < MIN_CAMERA_SIZE)
            return false;

        if (direction > 0)
            currentCameraSize += 1;


        int sideLength = currentCameraSize * 2 - 1;
        int columnCount = sideLength * 2;

        int startIndex = (MAX_CAMERA_SIZE - currentCameraSize) * ROW_COUNT + MAX_CAMERA_SIZE - currentCameraSize;

        for (int i = 0; i <= sideLength; i++)    // Top row
            nodeArray[startIndex + i].ToggleNodeVisible();

        for (int i = 2; i < columnCount; i++)   // Columns
            nodeArray[startIndex + (i / 2) * ROW_COUNT + (i + 1) % 2 * sideLength].ToggleNodeVisible();

        for (int i = 0; i <= sideLength; i++)    // Bottom row
            nodeArray[startIndex + sideLength * ROW_COUNT + i].ToggleNodeVisible();


        if (direction < 0)
            currentCameraSize -= 1;

        return true;
    }


    public void ResetNodes()
    {
        for (int i = 0; i < nodeArray.Length; i++)
            nodeArray[i].SetNodeBlocked(false);
    }

    public void FillNodes()
    {
        for (int i = 0; i < nodeArray.Length; i++)
            nodeArray[i].SetNodeBlocked(true);
    }


    public void AddVisibleNode(Node node)
    {
        visibleNodeList.Add(node);
    }

    public void RemoveVisibleNode(Node node)
    {
        visibleNodeList.Remove(node);
    }


    public void RandomizeEntityPosition(Entity entity)
    {
        List<Node> emptyNodes = new List<Node>();

        for (int i = 0; i < visibleNodeList.Count; i++)
        {
            Node node = visibleNodeList[i];
            if (!node.blocked && !node.GetBlocked())
                emptyNodes.Add(node);
        }

        if (emptyNodes.Count > 0)
        {
            int prevIndex = (entity.GetPosition().x + MAX_CAMERA_SIZE) % ROW_COUNT + (MAX_CAMERA_SIZE - entity.GetPosition().y) * ROW_COUNT;
            int newIndex = Random.Range(0, emptyNodes.Count);

            nodeArray[prevIndex].SetEntity(null);
            entity.SetPosition(Vector2Int.RoundToInt(emptyNodes[newIndex].transform.position));
            emptyNodes[newIndex].SetEntity(entity);
        }
    }


    public void SetEntityPosition(Entity entity, Vector2Int position)
    {
        int prevIndex = (entity.GetPosition().x + MAX_CAMERA_SIZE) % ROW_COUNT + (MAX_CAMERA_SIZE - entity.GetPosition().y) * ROW_COUNT;
        int newIndex = (position.x + MAX_CAMERA_SIZE) % ROW_COUNT + (MAX_CAMERA_SIZE - position.y) * ROW_COUNT;

        if (nodeArray[newIndex].GetBlocked())
        {
            Manager.instance.SelectEntity(nodeArray[newIndex].GetEntity());
            return;
        }

        nodeArray[prevIndex].SetEntity(null);
        nodeArray[newIndex].SetEntity(entity);

        entity.SetPosition(position);

        Manager.instance.SelectEntity(null);
    }


    public Entity GetEntity(Vector2Int position)
    {
        int index = (position.x + MAX_CAMERA_SIZE) % ROW_COUNT + (MAX_CAMERA_SIZE - position.y) * ROW_COUNT;
        return nodeArray[index].GetEntity();
    }
}
