using UnityEngine;

public class AgeManager
{
    private SoldierFactory currentFactory;

    public void SetAge(SoldierFactory factory)
    {
        currentFactory = factory;
    }

    public SoldierFactory GetFactory()
    {
        return currentFactory;
    }
}


