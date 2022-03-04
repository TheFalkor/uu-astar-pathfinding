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
    private bool blocked = false;
    private bool visible = true;


    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }

    public void SetNodeBlocked(bool blocked)
    {
        if (this.blocked == blocked)
            return;

        this.blocked = blocked;

        if (blocked)
            render.color = COLOR_BLOCKED;
        else
            render.color = COLOR_EMPTY;
    }

    public void ToggleNodeVisible()
    {
        visible = !visible;
        gameObject.SetActive(visible);
    }
}
