using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSoldier : Soldier
{
    
    public MeleeSoldier() : base(new MeleeAttack(), new SimpleMove())
    {
    }

    public override void Display()
    {
        
    }
}
