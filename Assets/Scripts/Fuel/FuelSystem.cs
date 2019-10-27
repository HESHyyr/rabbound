using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelSystem : MonoBehaviour
{
    [SerializeField] float fuelCapacity;
    [SerializeField] Slider fuelBar;

    FuelTank fuelTank;
    
    void Awake()
    {
        fuelTank = new FuelTank(fuelCapacity);
        fuelTank.RechargeFull();
    }

    public FuelTank GetFuelTank() {
        return fuelTank;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float fuelLevelPercentage = fuelTank.GetPercentage();
        //Debug.Log("Current Level: " + Mathf.Round(fuelLevelPercentage * 100) + "%");
        fuelBar.value = fuelLevelPercentage;
    }
}
