

public class BomStatus_BomAttack : BomStatus
{
    public override void Request(BomStatus cBomStatus)
    {
        cBomStatus.EnableAttack();
    }
}
