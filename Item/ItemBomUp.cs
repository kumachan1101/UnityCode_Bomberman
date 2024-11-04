public class ItemBomUp : ItemBomStatus
{
    protected override BomStatusType GetBomStatusType()
    {
        return BomStatusType.BomUp;
    }
}