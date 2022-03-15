using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


    public void GridSizeSlider(Slider slider)
    {
        Manager.instance.SetCameraSize((int)slider.value);
        slider.transform.Find("Value Text").GetComponent<Text>().text = "" + slider.value;
    }

    public void SimulationSpeedSlider(Slider slider)
    {
        Manager.instance.SetSimulationSpeed(slider.value);

        int value = (int)(slider.value * 10);
        string syntax = value / 10 + "." + value % 10 + "x";

        slider.transform.Find("Value Text").GetComponent<Text>().text = syntax;
    }
}
