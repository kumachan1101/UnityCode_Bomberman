public class BomConfigurationBomUpManager : BomConfigurationManagerBase
{
    public BomConfigurationBomUpManager() => configuration = new BomConfigurationBomUp();

    public override void Request(ReqType reqType){
        configuration.Request();
    }
}


public class BomConfigurationBomUp : BomConfigurationBase
{
    public BomConfigurationBomUp(){
        value = 3;
    }
    public override void Request()
    {
        int ivalue = (int)Get();
        ivalue++;
        value = ivalue;
    }
}
