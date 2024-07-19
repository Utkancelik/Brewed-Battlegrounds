using System;
using UnityEngine;

public class EraManager : MonoBehaviour
{
    public static event Action<int> OnEraChanged;

    private int currentEra = 1;
    public int CurrentEra => currentEra;

    public void SetEra(int era)
    {
        currentEra = era;
        OnEraChanged?.Invoke(currentEra);
    }
}
