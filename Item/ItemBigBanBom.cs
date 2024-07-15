public class ItemBigBanBom : Item{
    public override void Reflection(string objname){
        BomConfiguration cBomConfiguration = Library_Base.GetBomConfigurationFromObject(objname);
        if(null != cBomConfiguration){
            cBomConfiguration.SetBomKind(BOM_KIND.BOM_KIND_BIGBAN);
        }
    }
}
