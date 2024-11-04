public abstract class ItemBomStatus : Item
{
    protected abstract BomStatusType GetBomStatusType();

    public override void Reflection(string objname)
    {
        BomStatus cBomStatusCur = Library_Base.GetBomStatusFromObject(objname);
        PlayerBom cPlayerBom = Library_Base.GetPlayerBomFromObject(objname);
        BomStatus cBomStatusReq = cPlayerBom.CreateBomStatus(GetBomStatusType());
        cBomStatusReq.Request(cBomStatusCur);
    }
}
