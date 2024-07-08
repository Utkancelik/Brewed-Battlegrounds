public class FutureSoldierFactory : SoldierFactory
{
    public override Soldier CreateBasicUnit() => new PlayerInfantry(); // Replace with future equivalent
    public override Soldier CreateRangedUnit() => new PlayerArcher(); // Replace with future equivalent
    public override Soldier CreateAdvancedUnit() => new PlayerCavalry(); // Replace with future equivalent
}