using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class CashierClockIn : IWorkerTask
{
    public CheckoutObject checkout;

    public void StartTask(WorkerBrain brain)
    {
        brain.occupied = true;

        checkout = brain.occupiedObject.GetComponent<CheckoutObject>();

        brain.agent.SetDestination(checkout.cashierPosition.position);

        brain.UpdateWorkerText();
    }

    public void Execute(WorkerBrain brain)
    {
        if(brain.occupiedObject == null)
        {
            brain.SetTask(new WorkerIdle());
        }

        if(checkout.queue.Count > 0)
        {
            brain.transform.LookAt(checkout.queue[0].cBrain.transform.position);
        }
    }

    public void EndTask(WorkerBrain brain)
    {
        brain.occupied = false;
    }
}
