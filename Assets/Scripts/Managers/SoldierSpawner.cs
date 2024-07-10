using UnityEngine;

public class SoldierSpawner
{
    public SpawnArea PlayerSpawnArea { get; private set; }
    public SpawnArea EnemySpawnArea { get; private set; }

    public SoldierSpawner(SpawnArea playerSpawnArea, SpawnArea enemySpawnArea)
    {
        PlayerSpawnArea = playerSpawnArea;
        EnemySpawnArea = enemySpawnArea;
    }

    public void SpawnSoldier(GameObject soldierPrefab, bool isEnemy)
    {
        Vector3 spawnPosition = isEnemy ? EnemySpawnArea.GetRandomPosition() : PlayerSpawnArea.GetRandomPosition();
        GameObject newSoldier = GameObject.Instantiate(soldierPrefab, spawnPosition, Quaternion.identity);

        Debug.Log("Instantiated soldier: " + newSoldier.name);

        Soldier soldierScript = newSoldier.GetComponent<Soldier>();
        if (soldierScript == null)
        {
            Debug.LogError("Soldier component not found on instantiated prefab.");
            return;
        }
        soldierScript.IsEnemy = isEnemy;
        newSoldier.tag = isEnemy ? "EnemySoldier" : "PlayerSoldier";
    }
}