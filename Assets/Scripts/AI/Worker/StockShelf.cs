using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class StockShelf : IWorkerTask
{
    private bool inProcess;

    public void StartTask(WorkerBrain brain)
    {
        inProcess = false;
        brain.occupied = true;

        brain.agent.SetDestination(brain.occupiedObject.transform.position);
        if (brain.products.Count <= 0)
        {
            brain.occupiedObject.GetComponent<ShelveObject>().occupied = false;
            brain.SetTask(new WorkerIdle());
        }
    }

    public void Execute(WorkerBrain brain)
    {
        float dist = Vector3.Distance(brain.occupiedObject.transform.position, brain.transform.position);

        if (dist <= 2f && inProcess == false)
        {
            brain.StartCoroutine(FillShelve(brain));
        }
    }

    public IEnumerator FillShelve(WorkerBrain brain)
    {
        inProcess = true;

        ShelveObject shelve = brain.occupiedObject.GetComponent<ShelveObject>();

        int stockdiffrence = shelve.storageCapacity - shelve.products.Count;

        for (int i = 0; i < stockdiffrence; i++)
        {
            for (int z = 0; z < brain.products.Count; z++)
            {
                if(shelve.Type == brain.productLib.products[brain.products[z]].storageType)
                {
                    shelve.products.Add(brain.products[z]);

                    brain.products.RemoveAt(z);

                    shelve.FilledPercentage();

                    brain.UpdateWorkerText();

                    yield return new WaitForSeconds(.15f);

                    break;
                }
            }
        }
        shelve.occupied = false;

        inProcess = false;
        brain.SetTask(new WorkerIdle());
    }

    public void EndTask(WorkerBrain brain)
    {
        brain.occupied = false;
        brain.occupiedObject = null;
    }
}
