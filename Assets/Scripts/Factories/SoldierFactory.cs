using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class SoldierFactory : MonoBehaviour
{
    public static SoldierFactory Instance;

    [SerializeField] private GameObject meleeSoldierPrefab;
    [SerializeField] private GameObject rangedSoldierPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject CreateSoldier(SoldierType type)
    {
        GameObject soldier = null;

        switch (type)
        {
            case SoldierType.Melee:
                soldier = Instantiate(meleeSoldierPrefab);
                break;
            case SoldierType.Ranged:
                soldier = Instantiate(rangedSoldierPrefab);
                break;
        }

        return soldier;
    }
}*/


