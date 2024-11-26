using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using TMPro;

[System.Serializable]
public struct ObjectContainer
{
    public ObjectTypes type;

    public List<GameObject> objects;
}

public class ObjectManager : MonoBehaviour
{
    public ObjectContainer[] containers;

    private NavMeshSurface surface;

    public ProductLibrary productLibrary;

    private UIManager uiManager;

    [Space]
    [SerializeField] private Vector2Int customerSpawnRates;
    [SerializeField] private GameObject customer;
    public List<GameObject> Customers;
    [SerializeField] private float tickTime;
    private float currentTickTime;
    [Space]
    [SerializeField] private WorkerLibrary workerLib;
    public List<WorkerBrain> stockers;
    public List<WorkerBrain> cashiers;
    public List<GameObject> boxes;

    int payoutTimer = 12;
    int payoutId;

    public ReviewMeter meter;


    private void Awake()
    {
        payoutId = 0;

        uiManager = FindObjectOfType<UIManager>();

        containers = new ObjectContainer[System.Enum.GetValues(typeof(ObjectTypes)).Length];

        // initialize containers

        for (int i = 0; i < containers.Length; i++)
        {
            containers[i].type = (ObjectTypes)i;
            containers[i].objects = new List<GameObject>();
        }

        surface = GetComponent<NavMeshSurface>();

        surface.BuildNavMesh();

        currentTickTime = tickTime;

        meter = FindObjectOfType<ReviewMeter>();

    }

    private void Update()
    {
        currentTickTime -= Time.deltaTime;

        if(currentTickTime <= 0)
        {
            payoutId++;

            if(payoutId >= payoutTimer)
            {
                PayOutWorkers();
            }

            StartCoroutine(SpawnCustomer(Random.Range(meter.spawnRates[((int)meter.review)].x, meter.spawnRates[((int)meter.review)].y + 1)));

            CheckShelves();

            CheckCashiers();

            currentTickTime = tickTime;
        }
    }

    public CheckoutObject ShortestCheckOut()
    {
        int lineLenght = -1;
        GameObject best = null;

        foreach (GameObject checkout in containers[(int)ObjectTypes.checkout].objects)
        {
            CheckoutObject cObj = checkout.GetComponent<CheckoutObject>();

            if(lineLenght == -1 && best == null)
            {
                best = checkout;

                lineLenght = cObj.queue.Count;
            }
            else
            {
                if(cObj.queue.Count < lineLenght)
                {
                    best = checkout;

                    lineLenght = cObj.queue.Count;
                }
            }
        }

        return best.GetComponent<CheckoutObject>();
    }

    public void PayOutWorkers()
    {
        foreach (WorkerBrain worker in stockers)
        {
            uiManager.SpendMoney(workerLib.Workers[1].WorkerPrice);
        }

        foreach (WorkerBrain worker in cashiers)
        {
            uiManager.SpendMoney(workerLib.Workers[0].WorkerPrice);
        }

        payoutId = 0;
    }

    public void CheckShelves()
    {
        foreach (GameObject shelve in containers[(int)ObjectTypes.shelve].objects)
        {
            ShelveObject shelveObj = shelve.GetComponent<ShelveObject>();

            if(shelveObj.percentageRounded <= 50 && shelveObj.occupied == false)
            {
                foreach (WorkerBrain worker in stockers)
                {
                    if (worker.occupied == false && worker.products.Count > 0)
                    {
                        foreach (int productId in worker.products)
                        {
                            if(productLibrary.products[productId].storageType == shelveObj.Type)
                            {
                                shelveObj.occupied = true;

                                worker.occupiedObject = shelve;
                                worker.SetTask(new StockShelf());

                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    public void CheckCashiers()
    {
        foreach (GameObject checkout in containers[(int)ObjectTypes.checkout].objects)
        {
            CheckoutObject cObject = checkout.GetComponent<CheckoutObject>();

            if(cObject.isSelfCheckout == false && cObject.occupiedWorker == null)
            {
                foreach (WorkerBrain worker in cashiers)
                {
                    if(worker.occupied == false)
                    {
                        cObject.occupiedWorker = worker.gameObject;

                        worker.occupiedObject = checkout;
                        worker.SetTask(new CashierClockIn());

                        break;
                    }
                }
            }
        }
    }

    public IEnumerator SpawnCustomer(int amount)
    {
        int maxCustomers = (containers[(int)ObjectTypes.shelve].objects.Count + containers[(int)ObjectTypes.checkout].objects.Count) * 3;

        if (containers[(int)ObjectTypes.shelve].objects.Count != 0 && containers[(int)ObjectTypes.checkout].objects.Count != 0 && Customers.Count < maxCustomers)
        {
            for (int i = 0; i < amount; i++)
            {
                Instantiate(customer, transform.position, Quaternion.identity);

                yield return new WaitForSeconds(.25f);
            }
        }
    }

    public IEnumerator PlaceObject(ObjectTypes type, GameObject obj)
    {
        containers[(int)type].objects.Add(obj);

        yield return new WaitForEndOfFrame();

        surface.BuildNavMesh();
    }

    public IEnumerator RemoveObject(ObjectTypes type, GameObject obj)
    {
        containers[(int)type].objects.Remove(obj);

        foreach (Transform child in obj.transform)
        {
            child.gameObject.layer = 0;
        }

        yield return new WaitForEndOfFrame();

        surface.BuildNavMesh();

        Destroy(obj);
    }
}
