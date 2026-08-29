
public class BomConfigurationBomSpeedUpManager : BomConfigurationManagerBase
{
    public BomConfigurationBomSpeedUpManager() => configuration = BomConfigurationFactory.Create(ReqType.BomSpeedUp);
}



public class BomConfigurationBomSpeedUp : BomConfigurationBase
{
    public const int MaximumValue = 5;

    public BomConfigurationBomSpeedUp(){
        value = 1;
    }
    public override void Request()
    {
        int ivalue = (int)Get();
        if(ivalue >= MaximumValue){
            return;
        }
        ivalue++;
        value = ivalue;
    }
}
