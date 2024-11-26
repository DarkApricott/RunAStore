using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum CustomerType{
    normal,
    vip
}

public enum CustomerExperience
{
    positive,
    negative
}

public interface ICustomerTask
{
    public void StartTask(CustomerBrain brain);
    public void Execute(CustomerBrain brain);
    public void EndTask(CustomerBrain brain);
}

[RequireComponent(typeof(NavMeshAgent))]
public class CustomerBrain : MonoBehaviour
{
    public ICustomerTask currentTask;

    public CustomerType cType;

    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public ObjectManager oManager;
    [HideInInspector] public UIManager uiManager;

    public List<int> products = new List<int>();

    [Header("CustomerExperience")]
    [Range(-2, 2)]
    public int customerSatisfactory;
    [SerializeField] private SpriteRenderer emoticonRenderer;

    [Space]
    [SerializeField] private Sprite happySprite, midSprite, angrySprite;

    private CustomerTexturePicker picker;
    [SerializeField] private GameObject vipText;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        oManager = FindObjectOfType<ObjectManager>();
        uiManager = FindObjectOfType<UIManager>();

        picker = GetComponent<CustomerTexturePicker>();
        SetCustomer();

        oManager.Customers.Add(this.gameObject);
        SetTask(new BuyItem());
    }

    public void SetCustomer()
    {
        int customTypeRandom = Random.Range(1, 10);

        if(customTypeRandom == 5)
        {
            cType = CustomerType.vip;
            vipText.SetActive(true);
        }
        else
        {
            cType = CustomerType.normal;
            vipText.SetActive(false);
        }

        picker.pickMat(cType);
    }

    public void SetTask(ICustomerTask task)
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

    public IEnumerator ChangeCustomerExperience(CustomerExperience experience)
    {
        if(experience == CustomerExperience.positive)
        {
            if(cType == CustomerType.normal)
            {
                oManager.meter.reviewScore += 3;
            }
            else
            {
                oManager.meter.reviewScore += 12;
            }

            emoticonRenderer.sprite = happySprite;
        } else if(experience == CustomerExperience.negative)
        {
            if (cType == CustomerType.normal)
            {
                oManager.meter.reviewScore--;
            }
            else
            {
                oManager.meter.reviewScore -= 3;
            }
        }

        yield return new WaitForSeconds(1f);

        emoticonRenderer.sprite = null;
    }

    private void OnDrawGizmos()
    {
        if (agent != null && agent.destination != null)
        {
            Gizmos.color = Color.red;
            {
                // Draw lines joining each path corner
                Vector3[] pathCorners = agent.path.corners;

                for (int i = 0; i < pathCorners.Length - 1; i++)
                {
                    Gizmos.DrawLine(pathCorners[i], pathCorners[i + 1]);
                }

            }
        }
    }
}
