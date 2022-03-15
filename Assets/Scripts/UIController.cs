using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [Header("UI Variables")]
    private bool covering = false;


    [Header("Object References")]
    private GameObject blockInputCover;


    void Start()
    {
        blockInputCover = transform.GetChild(0).Find("Input Cover").gameObject;
        blockInputCover.SetActive(covering);
    }


    public void ToggleCover()
    {
        covering = !covering;

        blockInputCover.SetActive(covering);
    }


    public void GridRandomize()
    {
        Manager.instance.grid.NoisePattern();
    }

    public void GridFill()
    {
        Manager.instance.grid.FillNodes();
    }

    public void GridClear()
    {
        Manager.instance.grid.ClearNodes();
    }

    public void RandomizeEntityPositions()
    {
        Manager.instance.RandomizeEntityPositions();
    }


    public void GridSizeSlider(float value)
    {
        print(value);
    }
}
