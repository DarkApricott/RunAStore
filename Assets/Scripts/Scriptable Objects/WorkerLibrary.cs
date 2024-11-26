using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Worker
{
    public string WorkerName;
    public string WorkerJob;
    public int WorkerPrice;
    [Space]
    [Header("Visuals")]
    public GameObject prefab;
    public Sprite WorkerIcon;
}

[CreateAssetMenu(fileName = "Worker Library", menuName = "ScriptableObjects/WorkerLibrary", order = 1)]
public class WorkerLibrary : ScriptableObject
{
    public Worker[] Workers;
}
