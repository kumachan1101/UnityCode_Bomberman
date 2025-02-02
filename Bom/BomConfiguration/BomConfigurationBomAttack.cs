public class BomConfigurationBomAttackManager : BomConfigurationManagerBase
{
    public BomConfigurationBomAttackManager() => configuration = new BomConfigurationBomAttack();

    public override void Request(ReqType reqType){
        configuration.Request();
    }
}

public class BomConfigurationBomAttack : BomConfigurationBase
{
    public BomConfigurationBomAttack(){
        value = false;
    }
    public override void Request()
    {
        value = true;
    }
}
