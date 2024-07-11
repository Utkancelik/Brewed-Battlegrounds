using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgeManager : MonoBehaviour
{
    public static AgeManager Instance;

    private int playerAge = 1;
    private int enemyAge = 1;

    void Awake()
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

    public int PlayerAge => playerAge;
    public int EnemyAge => enemyAge;

    public void IncreasePlayerAge()
    {
        playerAge++;
        // Update player units and abilities based on new age
    }

    public void IncreaseEnemyAge()
    {
        enemyAge++;
        // Update enemy units and abilities based on new age
    }
}
