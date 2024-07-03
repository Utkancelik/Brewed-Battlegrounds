using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class MeleeAttack : MonoBehaviour ,IAttackBehavior
{
    public void Attack()
    {
        // Implementation melee attack
        Debug.Log("Melee attack");
    }
    
}
