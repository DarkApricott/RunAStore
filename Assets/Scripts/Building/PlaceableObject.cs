using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class PlaceableObject : MonoBehaviour
{
    public ObjectTypes type;
    [HideInInspector] public buildingTypes bType;
    private BuildingSystem system;
    private ObjectManager oManager;
    private UIManager uiMan;

    private GridCell gCell;

    private BoxCollider col;

    public int id;

    [SerializeField] private MeshRenderer renderer;

    [SerializeField] private List<int> ColorableMaterialsIds = new List<int>();

    private FlexibleColorPicker colorPicker;

    private EventSystem eventSystem;
    private Tutorial tutCS;

    private AudioSource source;
    [SerializeField] private AudioClip placeSound;

    private void Start()
    {
        oManager = FindObjectOfType<ObjectManager>();
        system = FindObjectOfType<BuildingSystem>();
        uiMan = FindObjectOfType<UIManager>();
        SetBuildType();

        col = GetComponent<BoxCollider>();
        col.enabled = false;

        colorPicker = FindObjectOfType<FlexibleColorPicker>();

        eventSystem = FindObjectOfType<EventSystem>();

        source = GetComponent<AudioSource>();

        tutCS = FindObjectOfType<Tutorial>();
    }

    private void LateUpdate()
    {
        if (system.mode != buildModes.build && system.isInBuildMode == true)
        {
            col.enabled = true;
        }
        else
        {
            col.enabled = false;
        }
    }

    public void PlaceObject(GridCell cell)
    {
        StartCoroutine(oManager.PlaceObject(type, gameObject));

        if (tutCS.DoingTutorial)
        {
            if (type == ObjectTypes.shelve)
            {
                tutCS.StepDone(0);
                tutCS.NextStep(2);
                tutCS.HelpingTexts(0);
            }
            else if (type == ObjectTypes.checkout)
            {
                tutCS.StepDone(4);
                tutCS.HelpingTexts(0);
            }
        }

        gCell = cell;

        source.PlayOneShot(placeSound);
    }

    public void RemoveObject(GridCell cell)
    {
        cell.RemoveObjectFromGrid(this.gameObject, bType);

        system.uiManager.GainMoney(system.buildingLibrary.buildObjects[id].BuildCashBack());

        StartCoroutine(oManager.RemoveObject(type, gameObject));
    }

    public void ChangeColor()
    {
        if (!uiMan.IsColorPicking)
        {
            foreach (int matId in ColorableMaterialsIds)
            {
                renderer.materials[matId].SetColor("_BaseColor", colorPicker.GetColor());
            }
        }
        else
        {
            foreach (int matId in ColorableMaterialsIds)
            {
                colorPicker.SetColor(renderer.materials[matId].GetColor("_BaseColor"));
            }

            uiMan.IsColorPicking = false;
        }
    }



    void SetBuildType()
    {
        switch (type)
        {
            case ObjectTypes.floor:
                bType = buildingTypes.floor;
                break;
            case ObjectTypes.wall:
                bType = buildingTypes.wall;
                break;
            case ObjectTypes.doors:
                bType = buildingTypes.wall;
                break;
            case ObjectTypes.shelve:
                bType = buildingTypes.interactable;
                break;
            case ObjectTypes.checkout:
                bType = buildingTypes.interactable;
                break;
        }
    }

    public void OnMouseDown()
    {
        if (eventSystem.IsPointerOverGameObject())
        {
            return;
        }

        if (system.isInBuildMode == true && system.mode == buildModes.destroy)
        {
            RemoveObject(gCell);
        }

        if (system.isInBuildMode == true && system.mode == buildModes.paint)
        {
            ChangeColor();
        }
    }
}
