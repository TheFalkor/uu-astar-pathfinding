using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity Variables")]
    private SpriteRenderer render;
    private Vector2Int position;


    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        position = Vector2Int.RoundToInt(transform.position);
    }


    public void SetPosition(Vector2Int position)
    {
        this.position = position;
        gameObject.transform.position = (Vector2)position;
        gameObject.SetActive(true);
    }


    public Vector2Int GetPosition()
    {
        return position;
    }


    public void SetSelected(bool active)
    {
        Color color = render.color;
        if (active)
            color.a = 0.5f;
        else
            color.a = 1;

        render.color = color;
    }
}