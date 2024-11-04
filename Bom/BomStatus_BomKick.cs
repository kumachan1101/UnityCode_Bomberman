
public class BomStatus_BomKick : BomStatus
{
    public override void Request(BomStatus cBomStatus)
    {
        cBomStatus.EnableKick();
    }

}
