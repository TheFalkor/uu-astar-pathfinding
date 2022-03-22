using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [Header("Cell Tools")]
    private Color COLOR_EMPTY = Color.white;
    private Color COLOR_BLOCKED = new Color(35 / 255f, 35 / 255f, 35 / 255f);
    private SpriteRenderer render;
    private Animator anim;
    private PathVisualizer pathVisualizerIn;
    private PathVisualizer pathVisualizerOut;
    public GameObject cellNodeVisual;


    [Header("Cell Variables")]
    private readonly List<Cell> neighborList = new List<Cell>();
    private Vector2Int position;
    private Vector2Int realPosition;
    private Entity occupyingEntity;
    private bool occupied = false;
    public bool blocked = false;
    public bool visible = true;


    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        realPosition = Vector2Int.RoundToInt(transform.position);

        cellNodeVisual = transform.GetChild(0).gameObject;
        pathVisualizerIn = transform.GetChild(1).GetComponent<PathVisualizer>();
        pathVisualizerOut = transform.GetChild(2).GetComponent<PathVisualizer>();

        cellNodeVisual.SetActive(false);
        SetNodeMarker();
    }


    public void AddNeighbor(Cell cell)
    {
        neighborList.Add(cell);
    }

    public List<Cell> GetNeighbors()
    {
        return neighborList;
    }


    public void SetCellBlocked(bool blocked)
    {
        if (this.blocked == blocked || this.occupied == true)
            return;

        this.blocked = blocked;
        ClearPath();

        if (blocked)
            render.color = COLOR_BLOCKED;
        else
            render.color = COLOR_EMPTY;

        if(this.visible)
            anim.Play("CellDrawn");
    }


    public void SetCellVisible(bool visible)
    {
        this.visible = visible;
        gameObject.SetActive(visible);

        if (occupied)
            occupyingEntity.gameObject.SetActive(visible);
    }


    public void ToggleCellVisible()
    {
        visible = !visible;
        gameObject.SetActive(visible);
        
        if (visible)
            Manager.instance.grid.AddVisibleCell(this);
        else
            Manager.instance.grid.RemoveVisibleCell(this);

        if(occupied)
            occupyingEntity.gameObject.SetActive(visible);
    }


    public void SetEntity(Entity entity)
    {
        occupyingEntity = entity;
        occupied = entity != null;
    }

    public Entity GetEntity()
    {
        return occupyingEntity;
    }


    public bool GetBlocked()
    {
        return occupied || blocked;
    }

    public bool IsWalkable()
    {
        return !blocked && visible;
    }


    public Vector2Int GetRealPosition()
    {
        return realPosition;
    }

    public Vector2Int GetPosition()
    {
        return position;
    }

    public void SetPosition(Vector2Int pos)
    {
        position = pos;
    }

    public void DrawPath(Vector2Int previous, Vector2Int next, int stamina)
    {
        pathVisualizerIn.DrawPath(previous, stamina > 0);
        pathVisualizerOut.DrawPath(next, stamina > 1);
    }

    public void SetNodeMarker()
    {
        cellNodeVisual.SetActive(true);
    }

    public void ClearPath()
    {
        cellNodeVisual.SetActive(false);
        pathVisualizerIn.ClearPath();
        pathVisualizerOut.ClearPath();
    }
}
