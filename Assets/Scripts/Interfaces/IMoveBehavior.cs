using UnityEngine;

public abstract class IMoveBehavior : MonoBehaviour
{
    public abstract void Move(Rigidbody2D rb, Vector2 targetPosition, float speed);
}
