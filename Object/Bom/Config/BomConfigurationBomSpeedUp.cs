
public class BomConfigurationBomSpeedUpManager : BomConfigurationManagerBase
{
    public BomConfigurationBomSpeedUpManager() => configuration = BomConfigurationFactory.Create(ReqType.BomSpeedUp);
}



public class BomConfigurationBomSpeedUp : BomConfigurationBase
{
    public BomConfigurationBomSpeedUp(){
        value = 1;
    }
    public override void Request()
    {
        int ivalue = (int)Get();
        if(ivalue >= 5){
            return;
        }
        ivalue++;
        value = ivalue;
    }
}
