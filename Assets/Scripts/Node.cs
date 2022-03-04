using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Color clearColor;
    public Color blockedColor;
    private bool blocked = false;


    public void ToggleNode()
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

    public void HideNode()
    {
        gameObject.SetActive(false);
    }

    public void ShowNode()
    {
        gameObject.SetActive(true);
    }
}
