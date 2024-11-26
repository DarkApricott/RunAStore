using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    private GameObject shelf;
    private GameObject kassa;
    private GameObject cashier;
    private GameObject cargo;
    private GameObject Filling;
    private BuildingSystem buildingSystem;
   

    public GameObject Shelfline;
    public GameObject Buildkassaline;
    public GameObject Hierecashierline;
    public GameObject Stockline;
    public GameObject stockedline;
    public GameObject background;
    public GameObject tutorial1;


    public GameObject tutorial;
    //public GameObject arrow;
    
    
    void Start()
    {
        buildingSystem = FindObjectOfType<BuildingSystem>();
        
    }

    
    void Update()
    {
        shelf = GameObject.FindGameObjectWithTag("Stockable");
        if (shelf != null && buildingSystem.mode == buildModes.none)
        {
            
            Shelfline.SetActive(true);
            tutorial.SetActive(true);
            Filling = GameObject.FindGameObjectWithTag("20%");
            if(Filling != null)
            {
                stockedline.SetActive(true);
                tutorial.SetActive(false);
            } 
        }
        kassa = GameObject.FindGameObjectWithTag("kassa");
        if (kassa != null && buildingSystem.mode == buildModes.none)
        {
            Buildkassaline.SetActive(true);
        }
        cashier = GameObject.FindGameObjectWithTag("Cashier");
        if (cashier != null)
        {
            Hierecashierline.SetActive(true);
            tutorial1.SetActive(false);
        }
        cargo = GameObject.FindGameObjectWithTag("Cargo");
        if (cargo != null)
        {
            tutorial.SetActive(true);
            Stockline.SetActive(true);
        }
        

        
    }
}
