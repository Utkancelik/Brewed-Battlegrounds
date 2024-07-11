using UnityEngine;

[RequireComponent(typeof(Soldier))]
public class NormalMove : IMoveBehavior
{
    public override void Move(Rigidbody2D rb, Vector3 targetPosition, float speed)
    {
        Vector3 direction = (targetPosition - (Vector3)rb.position).normalized;
        rb.velocity = direction * speed;
    }
}
