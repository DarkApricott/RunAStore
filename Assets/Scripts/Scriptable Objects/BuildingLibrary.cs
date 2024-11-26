using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum buildingTypes
{
    floor,
    wall,
    interactable
}

[System.Serializable]
public struct BuildObject
{
    public string name;
    public buildingTypes type;
    public bool IsLocked;
    public int lockedPrice;
    [Space]
    public int buildPrice;
    public Sprite Icon, TypeIcon;
    
    public int BuildCashBack()
    {
        return buildPrice / 2;
    }

    [Space]
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "Building Library", menuName = "ScriptableObjects/BuildingLibrary", order = 2)]
public class BuildingLibrary : ScriptableObject
{
    public BuildObject[] buildObjects;
}
