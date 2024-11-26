using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LeaveStore : ICustomerTask
{
    private Transform spawnPoint;

    public void StartTask(CustomerBrain brain)
    {
        spawnPoint = brain.oManager.transform;

        brain.agent.SetDestination(spawnPoint.position);
    }

    public void Execute(CustomerBrain brain)
    {
        float dist = Vector3.Distance(brain.transform.position, spawnPoint.position);

        if(dist <= 2f)
        {
            brain.oManager.Customers.Remove(brain.gameObject);
            UnityEngine.Object.Destroy(brain.gameObject);
        }
    }

    public void EndTask(CustomerBrain brain)
    {

    }
}
