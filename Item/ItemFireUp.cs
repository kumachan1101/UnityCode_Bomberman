
public class ItemFireUp : Item{

    public override void Reflection(string objname){
        BomConfiguration cBomConfiguration = Library_Base.GetBomConfigurationFromObject(objname);
        if(null != cBomConfiguration){
            cBomConfiguration.IncreaseExplosion();
        }
    }

}
