using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(IAttackBehavior))]
public class Soldier : MonoBehaviour
{
    public IAttackBehavior attackBehavior;
    protected IMoveBehavior moveBehavior;

    private void OnValidate()
    {
        attackBehavior = GetComponent<IAttackBehavior>();
    }


    private void Start()
    {
        Debug.Log(attackBehavior);
    }

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
    
    
}
