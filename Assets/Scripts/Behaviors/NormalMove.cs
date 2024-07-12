using UnityEngine;

[RequireComponent(typeof(Soldier))]
public class NormalMove : IMoveBehavior
{
    public override void Move(Rigidbody2D rb, Vector2 targetPosition, float speed)
    {
        Vector2 direction = (targetPosition - rb.position).normalized;
        rb.velocity = direction * speed;
    }
}