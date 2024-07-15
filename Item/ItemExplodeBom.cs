
public class ItemExplodeBom : Item{
    public override void Reflection(string objname){
        PlayerBom cPlayerBom = Library_Base.GetPlayerBomFromObject(objname);
        if(null != cPlayerBom){
            cPlayerBom.SetBomKind(BOM_KIND.BOM_KIND_EXPLODE);
        }
    }
}
