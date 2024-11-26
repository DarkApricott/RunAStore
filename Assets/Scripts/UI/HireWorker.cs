using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HireWorker : MonoBehaviour
{
    public int id;
    public WorkerLibrary lib;

    [SerializeField] private TMP_Text name, job, price, amount;
    [SerializeField] private Image sprite;

    private ObjectManager manager;
    private UIManager uiM;

    private void Start()
    {
        manager = FindObjectOfType<ObjectManager>();
        uiM = FindObjectOfType<UIManager>();

        name.text = lib.Workers[id].WorkerName;
        job.text = lib.Workers[id].WorkerJob;
        price.text = "€" + lib.Workers[id].WorkerPrice.ToString() + " p/m";
        sprite.sprite = lib.Workers[id].WorkerIcon;
        amount.text = "0";
    }

    public void Hire()
    {
        if (uiM.HasEnoughMoney(lib.Workers[id].WorkerPrice))
        {
            uiM.SpendMoney(lib.Workers[id].WorkerPrice); 
            Instantiate(lib.Workers[id].prefab, manager.transform.position, Quaternion.identity);
            StartCoroutine(UpdateWorkerCounters());
        }
    }

    private IEnumerator UpdateWorkerCounters()
    {
        yield return new WaitForSeconds(0.05f);

        if (job.text == "Cashier")
        {
            amount.text = manager.cashiers.Count.ToString();
        }
        else if (job.text == "Stocker")
        {
            amount.text = manager.stockers.Count.ToString();
        }
        else
        {
            //  amount.text = manager.cleaners.Count.ToString();
        }
    }

    public void Fire()
    {
        if(id == 0 && manager.cashiers.Count > 0)
        {
            Destroy(manager.cashiers[0].gameObject);
            manager.cashiers.RemoveAt(0);
        }

        if(id == 1 && manager.stockers.Count > 0)
        {
            manager.stockers[0].InventoryToBoxes();
            Destroy(manager.stockers[0].gameObject);
            manager.stockers.RemoveAt(0);
        }

        StartCoroutine(UpdateWorkerCounters());
    }
}
