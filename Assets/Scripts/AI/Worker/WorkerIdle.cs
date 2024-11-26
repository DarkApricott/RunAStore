using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorkerIdle : IWorkerTask
{
    public void StartTask(WorkerBrain brain)
    {
    }

    public void Execute(WorkerBrain brain)
    {
        if(brain.type == WorkerType.stocker)
        {
            if (brain.omanger.boxes.Count != 0)
            {
                brain.UpdateWorkerText();

                brain.agent.SetDestination(brain.omanger.boxes[0].transform.position);
            }
        }
    }
    public void EndTask(WorkerBrain brain)
    {
    }
}
