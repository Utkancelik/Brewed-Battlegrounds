using UnityEngine;

public class NormalMove : IMoveBehavior
{
    public void Move(Rigidbody2D rb, bool isEnemy, float speed)
    {
        Vector2 direction = isEnemy ? Vector2.left : Vector2.right;
        rb.velocity = direction * speed;
    }
}