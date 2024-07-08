using UnityEngine;

public class SoldierSpawner
{
    private SpawnArea playerSpawnArea;
    private SpawnArea enemySpawnArea;

    public SoldierSpawner(SpawnArea playerSpawnArea, SpawnArea enemySpawnArea)
    {
        this.playerSpawnArea = playerSpawnArea;
        this.enemySpawnArea = enemySpawnArea;
    }

    public void SpawnSoldier(GameObject soldierPrefab, bool isEnemy)
    {
        Vector3 spawnPosition = isEnemy ? enemySpawnArea.GetRandomPosition() : playerSpawnArea.GetRandomPosition();
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