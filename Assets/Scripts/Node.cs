using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [Header("Node Tools")]
    private Color COLOR_EMPTY = Color.white;
    private Color COLOR_BLOCKED = new Color(35 / 255f, 35 / 255f, 35 / 255f);
    private SpriteRenderer render;


    [Header("Node Variables")]
    private Entity occupyingEntity;
    private bool occupied = false;
    public bool blocked = false;
    private bool visible = true;


    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }


    public void SetNodeBlocked(bool blocked)
    {
        if (this.blocked == blocked || this.occupied == true)
            return;

        this.blocked = blocked;

        if (blocked)
            render.color = COLOR_BLOCKED;
        else
            render.color = COLOR_EMPTY;
    }


    public void SetNodeVisible(bool visible)
    {
        this.visible = visible;
        gameObject.SetActive(visible);

        if (occupied)
            occupyingEntity.gameObject.SetActive(visible);
    }


    public void ToggleNodeVisible()
    {
        visible = !visible;
        gameObject.SetActive(visible);
        
        if (visible)
            Manager.instance.grid.AddVisibleNode(this);
        else
            Manager.instance.grid.RemoveVisibleNode(this);

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
}
