using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum buildModes
{
    none,
    build,
    destroy,
    paint
}

public class BuildingSystem : MonoBehaviour
{
    public bool isInBuildMode;

    public buildModes mode;

    public BuildingLibrary buildingLibrary;

    public int buildObjId;

    public int rotAnchor;

    public GameObject placeableObject;

    [HideInInspector] public UIManager uiManager;
    [SerializeField] private BuildModeSwitcher furniture;

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateObject();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            SelectObject(buildObjId);
        }
    }

    public void RotateObject()
    {
        rotAnchor += 90;

        if (rotAnchor > 270)
        {
            rotAnchor = 0;
        }

        if (placeableObject != null) placeableObject.transform.rotation = Quaternion.Euler(0, rotAnchor, 0);
    }


    public void DisableBuildMode()
    {
        placeableObject = null;
        isInBuildMode = false;
    }

    public void SwitchAlternateMode(buildModes _mode)
    {
        mode = _mode;
        placeableObject = null;
    }

    public void SelectObject(int ID)
    {
        if (furniture.IsSwapped || buildObjId != ID)
        {
            if (buildObjId != ID)
            {
                furniture.IsSwapped = true;
            }

            isInBuildMode = true;

            mode = buildModes.build;

            buildObjId = ID;

            if (placeableObject != null)
            {
                Destroy(placeableObject);
            }

            if (uiManager.HasEnoughMoney(buildingLibrary.buildObjects[ID].buildPrice))
            {
                placeableObject = Instantiate(buildingLibrary.buildObjects[ID].prefab, transform.position - new Vector3(-1000, 1000, -1000), Quaternion.Euler(0, rotAnchor, 0));

                placeableObject.GetComponent<PlaceableObject>().id = ID;
            }
        }
    }
}
