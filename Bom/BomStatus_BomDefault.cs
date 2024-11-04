

public class BomStatus_BomDefault : BomStatus
{
    public override void Request(BomStatus cBomStatus)
    {
        cBomStatus.SetDefault();
    }
}
