using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Inspector References")]
    [SerializeField] private Sprite runSimulationSprite;
    [SerializeField] private Sprite endSimulationSprite;
    [Space]
    [SerializeField] private Image simulationButton;
    [SerializeField] private Text starsSoldText;
    [SerializeField] private Text staminaText;
    [Space]
    [SerializeField] private Button astarButton;
    [SerializeField] private Button jpsButton;

    [Header("UI Variables")]
    private bool covering = false;


    [Header("Object References")]
    private GameObject blockInputCover;


    void Start()
    {
        blockInputCover = transform.GetChild(1).Find("Input Cover").gameObject;
        blockInputCover.SetActive(covering);
    }

    public void UpdateStarCount(int count)
    {
        starsSoldText.text = "" + count;
    }

    public void UpdateStaminaCount(int count)
    {
        staminaText.text = "" + count;
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
        Manager.instance.grid.FillCells();
    }

    public void GridClear()
    {
        Manager.instance.grid.ClearCells();
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

    public void StaminaSlider(Slider slider)
    {
        Manager.instance.SetStamina((int)slider.value);
        slider.transform.Find("Value Text").GetComponent<Text>().text = "" + slider.value;
    }

    public void SetAlgorithm(int indexAlgorithm)
    {
        if(indexAlgorithm == 0)
        {
            jpsButton.interactable = true;
            astarButton.interactable = false;
            Manager.instance.SetAlgorithm(Algorithm.ASTAR);
        }
        else
        {
            jpsButton.interactable = false;
            astarButton.interactable = true;
            Manager.instance.SetAlgorithm(Algorithm.JPS);
        }
    }

    public void ToggleSimulation()
    {
        bool active = Manager.instance.ToggleSimulation();

        if (active)
            simulationButton.sprite = endSimulationSprite;
        else
            simulationButton.sprite = runSimulationSprite;
    }
}
