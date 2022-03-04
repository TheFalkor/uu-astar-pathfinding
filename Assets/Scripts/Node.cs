using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Color clearColor;
    public Color blockedColor;
    private bool blocked = false;
    private bool visible = true;


    public void ToggleNodeBlocked()
    {
        blocked = !blocked;

        if(blocked)
        {
            GetComponent<SpriteRenderer>().color = blockedColor;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = clearColor;
        }
    }

    public void SetNodeBlocked(bool blocked)
    {
        if (this.blocked == blocked)
            return;

        this.blocked = blocked;

        if (blocked)
        {
            GetComponent<SpriteRenderer>().color = blockedColor;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = clearColor;
        }
    }

    public void ToggleNodeVisible()
    {
        visible = !visible;
        gameObject.SetActive(visible);
    }
}
