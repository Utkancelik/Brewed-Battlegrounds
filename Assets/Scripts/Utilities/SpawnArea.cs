using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public Vector2 areaSize = new Vector2(5, 5);

    public Vector3 GetRandomPosition()
    {
        Vector3 position = transform.position;
        position.x += Random.Range(-areaSize.x / 2, areaSize.x / 2);
        position.y += Random.Range(-areaSize.y / 2, areaSize.y / 2);
        return position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}
