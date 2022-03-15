using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity Variables")]
    private SpriteRenderer render;
    private Vector2Int position;
    private Vector2Int homePosition;


    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        UpdatePosition();
    }


    public void SetPosition(Vector2Int position, bool saveHome)
    {
        this.position = position;
        gameObject.transform.position = (Vector2)position;
        gameObject.SetActive(true);

        if(saveHome)
            homePosition = position;
    }

    public void UpdatePosition()
    {
        position = Vector2Int.RoundToInt(transform.position);
        transform.position = (Vector2)position;
    }

    public Vector2Int GetPosition()
    {
        return position;
    }

    public Vector2Int GetHomePosition()
    {
        return homePosition;
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
