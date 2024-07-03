using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedSoldier : Soldier
{
    public RangedSoldier() : base(new RangedAttack(), new SimpleMove())
    {
    }

    public override void Display()
    {
        throw new System.NotImplementedException();
    }
}
