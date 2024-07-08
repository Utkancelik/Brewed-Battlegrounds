public class MedievalSoldierFactory : SoldierFactory
{
    public override Soldier CreateBasicUnit() => new PlayerInfantry();
    public override Soldier CreateRangedUnit() => new PlayerArcher();
    public override Soldier CreateAdvancedUnit() => new PlayerCavalry();
}