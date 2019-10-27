using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelTank
{
    const float EMPTY_THRESHOLD = 0;
    float capacity;
    float currentLevel;
    

    public float GetCapacity() {
        return capacity;
    }

    public FuelTank(float capacity) {
        currentLevel = 0;
        this.capacity = capacity;
    }

    public void RechargeFull() {
        Recharge(capacity);
    }

    public void Recharge(float amount) {
        currentLevel = Mathf.Min(currentLevel + amount, capacity);
    }

    public void Drain(float amount) {
        //currentLevel = Mathf.Max(0, currentLevel - amount);
        currentLevel -= amount;
    }

    public void DrainAll() {
        currentLevel = 0;
    }

    public float GetLevel() {
        return currentLevel;
    }

    public float GetPercentage() {
        return currentLevel / capacity;
    }

    public void SetCapacity(float newCapacity, bool scaling) {
        if (scaling) {
            currentLevel = newCapacity * GetPercentage();
        }
        capacity = newCapacity;
    }

    public bool isEmpty() {
        return currentLevel < EMPTY_THRESHOLD;
    }
}
