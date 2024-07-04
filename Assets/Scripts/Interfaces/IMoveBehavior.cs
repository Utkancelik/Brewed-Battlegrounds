using UnityEngine;
public interface IMoveBehavior
{
    void Move(Rigidbody2D rb, bool isEnemy, float speed);
}