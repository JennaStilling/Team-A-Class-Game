using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShopUpgrade_CoinMultiplier : ShopItem
{
    [SerializeField]
    EmployeeEnemyManager _employeeEnemyManager;
    private int upgradeLevel = 0;
    public void Start()
    {
        Debug.Log("test");
        upgradeLevel = 0;
        _cost = 100;
    }

    public override void PurchaseEffect()
    {
        //_employeeEnemyManager.maxTokensProp++;
        //_cost = CalculateItemCost(upgradeLevel++ /*_employeeEnemyManager.maxTokensProp - 1*/);
    }

    private int CalculateItemCost(int upgradeLevel) 
    {
        return 100 * (int) Math.Pow(2,upgradeLevel);
    }
}
