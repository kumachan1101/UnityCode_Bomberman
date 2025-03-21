/*
public class BomConfigurationBomKickManager : BomConfigurationManagerBase
{
    public BomConfigurationBomKickManager() => configuration = new BomConfigurationBomKick();

    public override void Request(ReqType reqType){
        configuration.Request();
    }
}
*/

public class BomConfigurationBomKickManager : BomConfigurationManagerBase
{
    public BomConfigurationBomKickManager() => configuration = BomConfigurationFactory.Create(ReqType.BomKick);
}
public class BomConfigurationBomKick : BomConfigurationBase
{
    public BomConfigurationBomKick(){
        value = false;
    }
    
    public override void Request()
    {
        value = true;
    }
}
