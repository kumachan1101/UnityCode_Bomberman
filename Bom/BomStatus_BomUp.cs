

public class BomStatus_BomUp : BomStatus
{
    public override void Request(BomStatus cBomStatus)
    {
        cBomStatus.IncreaseBom();
    }
}
