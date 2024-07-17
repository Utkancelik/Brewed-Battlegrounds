using UnityEngine;

[RequireComponent(typeof(Soldier))]
public class NormalMove : IMoveBehavior
{
    public override void Move(Rigidbody2D rb, Vector2 targetPosition, float speed)
    {
        rb.velocity = (targetPosition - rb.position).normalized * speed;
    }
}
