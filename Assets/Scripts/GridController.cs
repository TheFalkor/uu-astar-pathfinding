using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController
{
    [Header("Grid Settings")]
    private const int MIN_CAMERA_SIZE = 5;
    private const int MAX_CAMERA_SIZE = 15;


    [Header("Grid Variables")]
    private const int ROW_COUNT = MAX_CAMERA_SIZE * 2;
    private int currentCameraSize;
    private readonly Cell[] cellArray = new Cell[ROW_COUNT * ROW_COUNT];
    private readonly List<Cell> visibleCellList = new List<Cell>();


    public GridController(GameObject cellPrefab, int startSize)
    {
        currentCameraSize = startSize;
        Transform cellParent = new GameObject("Cells").transform;

        int minVisibleIndex = (MAX_CAMERA_SIZE - startSize) * ROW_COUNT;
        int maxVisibleIndex = cellArray.Length - minVisibleIndex - 1;
        for (int i = 0; i < cellArray.Length; i++)
        {
            Cell cell = Object.Instantiate(cellPrefab, new Vector2(-MAX_CAMERA_SIZE + i % ROW_COUNT, MAX_CAMERA_SIZE - i / ROW_COUNT), Quaternion.identity, cellParent).GetComponent<Cell>();
            cell.SetPosition(new Vector2Int(i % ROW_COUNT, i / ROW_COUNT));
            if (i > minVisibleIndex && i < maxVisibleIndex)
            {
                if (i % ROW_COUNT >= MAX_CAMERA_SIZE - startSize && i % ROW_COUNT < ROW_COUNT - MAX_CAMERA_SIZE + startSize)
                {
                    cell.SetCellVisible(true);
                    visibleCellList.Add(cell);
                }
                else
                    cell.SetCellVisible(false);
            }
            else
                cell.SetCellVisible(false);

            cellArray[i] = cell;
        }

        for (int i = 0; i < cellArray.Length; i++)
        {
            Cell cell = cellArray[i];

            if (i / ROW_COUNT != 0)
            {
                if (i % ROW_COUNT != 0)
                    cell.AddNeighbor(cellArray[i - ROW_COUNT - 1]);    // NW

                cell.AddNeighbor(cellArray[i - ROW_COUNT]);            // N

                if (i % ROW_COUNT != ROW_COUNT - 1)
                    cell.AddNeighbor(cellArray[i - ROW_COUNT + 1]);    // NE
            }

            if (i % ROW_COUNT != ROW_COUNT - 1)
                cell.AddNeighbor(cellArray[i + 1]);                    // E

            if (i / ROW_COUNT != ROW_COUNT - 1)
            {
                if (i % ROW_COUNT != ROW_COUNT - 1)
                    cell.AddNeighbor(cellArray[i + ROW_COUNT + 1]);    // SE

                cell.AddNeighbor(cellArray[i + ROW_COUNT]);            //S

                if (i % ROW_COUNT != 0)
                    cell.AddNeighbor(cellArray[i + ROW_COUNT - 1]);    // SW
            }

            if (i % ROW_COUNT != 0)
                cell.AddNeighbor(cellArray[i - 1]);                    // W

        }
    }


    public void SetCellBlocked(Vector2Int position, bool blocked)
    {
        if (position.x < -currentCameraSize || position.x >= currentCameraSize || position.y <= -currentCameraSize || position.y > currentCameraSize)
            return;

        int index = (position.x + MAX_CAMERA_SIZE) % ROW_COUNT + (MAX_CAMERA_SIZE - position.y) * ROW_COUNT;
        cellArray[index].SetCellBlocked(blocked);
    }


    public bool SetGridSize(int targetGridSize)
    {
        if (targetGridSize > MAX_CAMERA_SIZE || targetGridSize < MIN_CAMERA_SIZE)
            return false;



        while (targetGridSize != currentCameraSize)
        {
            if (targetGridSize > currentCameraSize)
                currentCameraSize += 1;


            int sideLength = currentCameraSize * 2 - 1;
            int columnCount = sideLength * 2;

            int startIndex = (MAX_CAMERA_SIZE - currentCameraSize) * ROW_COUNT + MAX_CAMERA_SIZE - currentCameraSize;

            for (int i = 0; i <= sideLength; i++)    // Top row
                cellArray[startIndex + i].ToggleCellVisible();

            for (int i = 2; i < columnCount; i++)   // Columns
                cellArray[startIndex + (i / 2) * ROW_COUNT + (i + 1) % 2 * sideLength].ToggleCellVisible();

            for (int i = 0; i <= sideLength; i++)    // Bottom row
                cellArray[startIndex + sideLength * ROW_COUNT + i].ToggleCellVisible();


            if (targetGridSize < currentCameraSize)
                currentCameraSize -= 1;
        }
        

        return true;
    }


    public void ClearCells()
    {
        for (int i = 0; i < cellArray.Length; i++)
            cellArray[i].SetCellBlocked(false);
    }

    public void FillCells()
    {
        for (int i = 0; i < cellArray.Length; i++)
            cellArray[i].SetCellBlocked(true);
    }

    public void NoisePattern()
    {
        ClearCells();
        for (int i = 0; i < cellArray.Length; i++)
        {
            if (Random.Range(0, 100) < 40)
                cellArray[i].SetCellBlocked(true);
        }
    }

    public void ClearDebug()
    {
        for (int i = 0; i < cellArray.Length; i++)
            cellArray[i].ClearPath();
    }


    public void AddVisibleCell(Cell cell)
    {
        visibleCellList.Add(cell);
    }

    public void RemoveVisibleCell(Cell cell)
    {
        visibleCellList.Remove(cell);
    }


    public void RandomizeEntityPosition(Entity entity, bool save)
    {
        List<Cell> emptyCells = new List<Cell>();

        for (int i = 0; i < visibleCellList.Count; i++)
        {
            Cell cell = visibleCellList[i];
            if (!cell.GetBlocked())
                emptyCells.Add(cell);
        }

        if (emptyCells.Count > 0)
        {
            int prevIndex = (entity.GetPosition().x + MAX_CAMERA_SIZE) % ROW_COUNT + (MAX_CAMERA_SIZE - entity.GetPosition().y) * ROW_COUNT;
            int newIndex = Random.Range(0, emptyCells.Count);
            if (save)
            {
                cellArray[prevIndex].SetEntity(null);
                emptyCells[newIndex].SetEntity(entity);
            }
            entity.SetPosition(Vector2Int.RoundToInt(emptyCells[newIndex].transform.position), save);
        }
    }


    public void SetEntityPosition(Entity entity, Vector2Int position)
    {
        int prevIndex = (entity.GetPosition().x + MAX_CAMERA_SIZE) % ROW_COUNT + (MAX_CAMERA_SIZE - entity.GetPosition().y) * ROW_COUNT;
        int newIndex = (position.x + MAX_CAMERA_SIZE) % ROW_COUNT + (MAX_CAMERA_SIZE - position.y) * ROW_COUNT;

        if (cellArray[newIndex].GetBlocked() || !cellArray[newIndex].visible)
        {
            Manager.instance.SelectEntity(cellArray[newIndex].GetEntity());
            return;
        }

        cellArray[prevIndex].SetEntity(null);
        cellArray[newIndex].SetEntity(entity);

        entity.SetPosition(position, true);

        Manager.instance.SelectEntity(null);
    }


    public Entity GetEntity(Vector2Int position)
    {
        if (position.x < -currentCameraSize || position.x >= currentCameraSize || position.y <= -currentCameraSize || position.y > currentCameraSize)
            return null;

        int index = (position.x + MAX_CAMERA_SIZE) % ROW_COUNT + (MAX_CAMERA_SIZE - position.y) * ROW_COUNT;
        return cellArray[index].GetEntity();
    }

    public Cell GetCellUsingGrid(int x, int y)
    {
        if (x < 0 || x >= ROW_COUNT || y < 0 || y >= ROW_COUNT)
            return null;

        int index = x % ROW_COUNT + y * ROW_COUNT;
        return cellArray[index];
    }

    public Cell GetCellUsingReal(Vector2Int position)
    {
        if (position.x < -currentCameraSize || position.x >= currentCameraSize || position.y <= -currentCameraSize || position.y > currentCameraSize)
        return null;

        int index = (position.x + MAX_CAMERA_SIZE) % ROW_COUNT + (MAX_CAMERA_SIZE - position.y) * ROW_COUNT;
        return cellArray[index];
    }


    public bool WalkableAt(int x, int y)
    {
        if (x < 0 || x >= ROW_COUNT || y < 0 || y >= ROW_COUNT)
            return false;

        int index = x % ROW_COUNT + y * ROW_COUNT;
        return cellArray[index].IsWalkable();
    }
}
