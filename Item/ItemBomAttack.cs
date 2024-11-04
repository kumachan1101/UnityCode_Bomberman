public class ItemBomAttack : ItemBomStatus
{
    protected override BomStatusType GetBomStatusType()
    {
        return BomStatusType.BomAttack;
    }
}
