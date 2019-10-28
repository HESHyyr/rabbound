using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelSystem : MonoBehaviour
{
    [SerializeField] float fuelCapacity;
    [SerializeField] Slider fuelBar;

    FuelTank fuelTank;
    bool recharging;
    float rechargeRate;

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
        if (recharging)
        {
            fuelTank.Recharge(rechargeRate * Time.deltaTime);
            recharging = !fuelTank.isFull();
        }
        fuelBar.value = fuelTank.isEmpty() ? 0 : fuelTank.GetPercentage();
    }

    public void Drain(float amount) {
        fuelTank.Drain(amount);
    }

    public void RechargeAllOvertime(float time) {
        recharging = true;
        rechargeRate = (100 - fuelTank.GetLevel()) / time;
    }
}
