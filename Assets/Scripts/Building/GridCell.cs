using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

[System.Serializable]
public struct rotationField
{
    public int rotAnchor;
    public GameObject obj;
}

[System.Serializable]
public class placeField
{
    public rotationField[] objects = new rotationField[4];

    public void SetStartRotation()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].rotAnchor = (i * 90);
        }
    }
}

public class GridCell : MonoBehaviour
{
    private BuildingSystem system;

    [SerializeField] private GameObject indicator;

    private bool canPlaceObject;

    [SerializeField] private placeField[] gridPlaceTypes = new placeField[Enum.GetNames(typeof(buildingTypes)).Length];

    private EventSystem eventSystem;

    private void Start()
    {
        system = FindObjectOfType<BuildingSystem>();

        foreach (placeField item in gridPlaceTypes)
        {
            item.SetStartRotation();
        }

        eventSystem = FindObjectOfType<EventSystem>();
    }

    private void Update()
    {
        if(canPlaceObject == true && Input.GetMouseButton(0) && system.placeableObject != null)
        {
            CheckGrid();
        }
    }

    public void CheckGrid()
    {
        PlaceableObject po = system.placeableObject.GetComponent<PlaceableObject>();

        if(po.bType != buildingTypes.floor)
        {
            for (int i = 0; i < gridPlaceTypes[(int)po.bType].objects.Length; i++)
            {
                if (gridPlaceTypes[(int)po.bType].objects[i].rotAnchor == system.rotAnchor)
                {
                    if(gridPlaceTypes[(int)po.bType].objects[i].obj == null)
                    {
                        gridPlaceTypes[(int)po.bType].objects[i].obj = system.placeableObject;

                        PlaceObjectOnGrid(po);
                    }
                }
            }
        } else if (gridPlaceTypes[(int)po.bType].objects[0].obj == null)
        {
            gridPlaceTypes[(int)po.bType].objects[0].obj = system.placeableObject;
            PlaceObjectOnGrid(po);
        }
    }

    public void PlaceObjectOnGrid(PlaceableObject po)
    {
        po.PlaceObject(this);

        system.uiManager.SpendMoney(system.buildingLibrary.buildObjects[system.buildObjId].buildPrice);

        system.placeableObject = null;

        system.SelectObject(system.buildObjId);

        SetObjectOnGrid(true);
    }

    private void OnMouseOver()
    {
        if(system.isInBuildMode == true && system.mode == buildModes.build)
        {
            if (eventSystem.IsPointerOverGameObject())
            {
                SetObjectOnGrid(false);
            }
            else
            {
                SetObjectOnGrid(true);
            }
        }
    }

    private void SetObjectOnGrid(bool state)
    {
        if(state == true && system.isInBuildMode == true)
        {
            canPlaceObject = true;
            indicator.SetActive(true);
            if (system.placeableObject != null) system.placeableObject.transform.position = this.transform.position;
        } else if (state == false || system.isInBuildMode == false)
        {
            canPlaceObject = false;
            indicator.SetActive(false);
            if (system.placeableObject != null) system.placeableObject.transform.position = transform.position - new Vector3(-1000, 1000, -1000);
        }
    }

    public void RemoveObjectFromGrid(GameObject obj, buildingTypes bType)
    {
        if (bType != buildingTypes.floor)
        {
            for (int i = 0; i < gridPlaceTypes[(int)bType].objects.Length; i++)
            {
                if (gridPlaceTypes[(int)bType].objects[i].obj == obj)
                {
                    gridPlaceTypes[(int)bType].objects[i].obj = null;
                }
            }
        }
        else if (gridPlaceTypes[(int)bType].objects[0].obj != null)
        {
            gridPlaceTypes[(int)bType].objects[0].obj = null;
        }
    }

    private void OnMouseExit()
    {
        SetObjectOnGrid(false);
    }
}
