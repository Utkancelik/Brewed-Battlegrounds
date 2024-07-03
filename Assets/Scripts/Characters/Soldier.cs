using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Soldier 
{
    protected IAttackBehavior attackBehavior;
    protected IMoveBehavior moveBehavior;

    public Soldier(IAttackBehavior attackBehavior, IMoveBehavior moveBehavior)
    {
        this.attackBehavior = attackBehavior;
        this.moveBehavior = moveBehavior;
    }

    public void PerformAttack()
    {
        attackBehavior.Attack();
    }

    public void PerformMove()
    {
        moveBehavior.Move();
    }
    
    public abstract void Display();  // for each soldier graph and animations
}
