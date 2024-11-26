using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public enum WorkerType
{
    stocker,
    cashier
}

public interface IWorkerTask
{
    public void StartTask(WorkerBrain brain);
    public void Execute(WorkerBrain brain);
    public void EndTask(WorkerBrain brain);
}

[RequireComponent(typeof(NavMeshAgent))]
public class WorkerBrain : MonoBehaviour
{
    public WorkerType type;
    [HideInInspector] public ObjectManager omanger;
    private SpawnBoxes spawnBoxesCS;

    public IWorkerTask currentTask;
    public bool occupied;
    public GameObject occupiedObject;

    public List<int> products;

    [HideInInspector] public NavMeshAgent agent;
    [SerializeField] public ProductLibrary productLib;

    public TMP_Text workerInfo;

    [SerializeField] private GameObject canvas;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        omanger = FindObjectOfType<ObjectManager>();

        spawnBoxesCS = FindObjectOfType<SpawnBoxes>();
        
        if(type == WorkerType.stocker)
        {
            omanger.stockers.Add(this);
        } else if(type == WorkerType.cashier)
        {
            omanger.cashiers.Add(this);
        }

        SetTask(new WorkerIdle());

        UpdateWorkerText();
    }

    public void SetTask(IWorkerTask task)
    {
        if (currentTask != null)
        {
            currentTask.EndTask(this);

            currentTask = task;
            currentTask.StartTask(this);
        }
        else
        {
            currentTask = task;
            currentTask.StartTask(this);
        }
    }

    public void Update()
    {
        currentTask.Execute(this);
    }

    public void UpdateWorkerText()
    {
        if (type == WorkerType.stocker)
        {
            workerInfo.text = products.Count.ToString();

            if (products.Count == 0)
            {
                canvas.SetActive(true);
                workerInfo.text = "I need stock to fill shelves... \n <color=red>" + products.Count.ToString() + "</color>";
            }
            else
            {
                canvas.SetActive(false);
                workerInfo.text = products.Count.ToString();
            }
        }

        if (type == WorkerType.cashier)
        {
            if (occupied == true)
            {
                canvas.SetActive(false);
                workerInfo.text = "";
            }
            else
            {
                canvas.SetActive(true);
                workerInfo.text = "I need a checkout to work... :(";
            }
        }
    }

    public IEnumerator SayMessage(string msg, float time)
    {
        canvas.SetActive(true);
        workerInfo.text = msg;

        yield return new WaitForSeconds(time);

        UpdateWorkerText();
    }

    public void InventoryToBoxes()
    {
        if (products.Count > 0)
        {
            spawnBoxesCS.ListNewProducts = products;
            spawnBoxesCS.SortProducts(ref spawnBoxesCS.ListNewProducts, products.Count);
        }
    }
}
