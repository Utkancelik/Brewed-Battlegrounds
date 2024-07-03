using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : IMoveBehavior
{
    public void Move()
    {
        // Simple forward movement
        Debug.Log("Moving forward");
    }
}
