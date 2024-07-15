
public class ItemBomUp : Item{
    
    public override void Reflection(string objname){
        BomStatus cBomStatus = Library_Base.GetBomStatusFromObject(objname);
        if(null != cBomStatus){
            cBomStatus.IncreaseBom();
        }

    }


}
