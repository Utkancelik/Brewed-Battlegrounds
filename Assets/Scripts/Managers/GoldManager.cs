using System;
using UnityEngine;

[Serializable]
public class GoldManager
{
    private float goldProductionRate = 50f; // Gold per second

    public void ProduceGold(ref int gold)
    {
        gold += Mathf.FloorToInt(goldProductionRate * Time.deltaTime);
    }
}
