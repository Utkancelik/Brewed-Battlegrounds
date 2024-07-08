public abstract class SoldierFactory
{
    public abstract Soldier CreateBasicUnit();
    public abstract Soldier CreateRangedUnit();
    public abstract Soldier CreateAdvancedUnit();
}