using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    [Header("Path Tools")]
    private Color COLOR_CANREACH = new Color(65 / 255f, 180 / 255f, 65 / 255f);
    private Color COLOR_NOSTAMINA = new Color(180 / 255f, 65 / 255f, 65 / 255f);
    private SpriteRenderer render;
    private Vector2Int position;


    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        position = Vector2Int.RoundToInt(transform.position);
        gameObject.SetActive(false);
    }

    public void DrawPathNew(Vector2Int direction, bool enoughStamina)
    {
        if (enoughStamina)
            render.color = COLOR_CANREACH;
        else
            render.color = COLOR_NOSTAMINA;

        gameObject.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y - position.y, direction.x - position.x) * Mathf.Rad2Deg);

        gameObject.SetActive(true);
    }

    public void ClearPath()
    {
        gameObject.SetActive(false);
    }
}
