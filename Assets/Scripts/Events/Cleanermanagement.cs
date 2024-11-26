using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Cleanermanagement : MonoBehaviour
{
    [SerializeField] private GameObject Barf;
    private float SpawnTime;

    private ObjectManager oManager;

    public Vector2 spawnRange = new Vector2(5, 35);
    private float spawnMaxTime;
    
    void Start()
    {
        oManager = FindObjectOfType<ObjectManager>();
        spawnMaxTime = Random.Range(spawnRange.x, spawnRange.y);
    }

    // Update is called once per frame
    private void Update()
    {
        SpawnTime += Time.deltaTime;
        if (SpawnTime >= spawnMaxTime)
        {
            SpawnBarf();
            SpawnTime = 0;
            spawnMaxTime = Random.Range(spawnRange.x, spawnRange.y);
        }
    }
    private void FixedUpdate()
    {
        
    }

    private void SpawnBarf()
    {
        if(oManager.Customers.Count > 0)
        {
            /*float spawnX = Random.Range(-8f, 8f);
            float spawnZ = Random.Range(-8f, 8f);*/

            int customerBarfId = Random.Range(0, oManager.Customers.Count);

            Instantiate(Barf, transform.position + new Vector3(oManager.Customers[customerBarfId].transform.position.x, 0.1f, oManager.Customers[customerBarfId].transform.position.z), Quaternion.identity);
        }
    }
}
