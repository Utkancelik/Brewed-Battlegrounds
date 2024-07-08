using System;
using UnityEngine;

[Serializable]
public class GoldManager
{
    public int goldProductionRate = 5;
    private float goldTimer;

    public void ProduceGold(ref int gold)
    {
        goldTimer += Time.deltaTime;
        if (goldTimer >= 1f)
        {
            gold += goldProductionRate;
            goldTimer = 0f;
        }
    }
}

