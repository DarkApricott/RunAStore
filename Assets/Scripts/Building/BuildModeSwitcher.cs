using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildModeSwitcher : MonoBehaviour
{
    private BuildingSystem system;
    [SerializeField] private buildModes mode;
    public bool IsSwapped;

    private void Awake()
    {
        system = FindObjectOfType<BuildingSystem>();
    }

    public void SetMode()
    {
        if (system.mode != mode)
        {
            IsSwapped = true;
        }

        system.SwitchAlternateMode(mode);

        if (mode != buildModes.none && IsSwapped)
        {
            system.isInBuildMode = true;
        }
    }

    public void SwapMode()
    {
        IsSwapped = !IsSwapped;

        if (!IsSwapped)
        {
            system.DisableBuildMode();
            system.SwitchAlternateMode(buildModes.none);
        }
    }
}
