using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    [Header("Inspector References")]
    public Sprite straightPath;
    public Sprite turnedPath;


    [Header("Path Tools")]
    private Color COLOR_CANREACH = new Color(220 / 255f, 75 / 255f, 75 / 255f);
    private Color COLOR_NOSTAMINA = new Color(220 / 255f, 75 / 255f, 75 / 255f, 150 / 255f);
    private SpriteRenderer render;
    private Vector2Int position;



    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        position = Vector2Int.RoundToInt(transform.position);
        gameObject.SetActive(false);
    }

    public void SetPath(Vector2Int previous, Vector2Int next, bool enoughStamina)
    {
        if (enoughStamina)
            render.color = COLOR_CANREACH;
        else
            render.color = COLOR_NOSTAMINA;

        Vector2Int deltaPosition = previous - next;

        int prevDeltaX = previous.x - position.x;
        int prevDeltaY = previous.y - position.y;

        int nextDeltaX = next.x - position.x;
        int nextDeltaY = next.y - position.y;


        if (-prevDeltaX == nextDeltaX)
        {
            render.sprite = straightPath;

            if (prevDeltaX != 0)
                gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
            else
                gameObject.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        else
        {
            int x = prevDeltaX == 0 ? nextDeltaX : prevDeltaX;
            int y = prevDeltaY == 0 ? nextDeltaY : prevDeltaY;

            print(Mathf.Asin(-1) + " || " + Mathf.Asin(1));
            print(x + ", " + y);
            if (x != y)
                gameObject.transform.eulerAngles = new Vector3(0, 0, Mathf.Acos(x) * Mathf.Rad2Deg);
            else
                gameObject.transform.eulerAngles = new Vector3(0, 0, Mathf.Asin(y) * Mathf.Rad2Deg);

            //x1, y-1 = 0
            //x1, y1 = 90
            //x-1, y1 = 180
            //x-1, y-1 = -90
            render.sprite = turnedPath;
        }



        gameObject.SetActive(true);
    }

    public void ClearPath()
    {
        gameObject.SetActive(false);
    }


}
