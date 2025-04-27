/*
public class BomConfigurationBomAttackManager : BomConfigurationManagerBase
{
    public BomConfigurationBomAttackManager() => configuration = new BomConfigurationBomAttack();

    public override void Request(ReqType reqType){
        configuration.Request();
    }
}
*/
public class BomConfigurationBomAttackManager : BomConfigurationManagerBase
{
    public BomConfigurationBomAttackManager() => configuration = new BomConfigurationBomAttackDefault();
}

public class BomConfigurationBomAttackDefault : BomConfigurationBase
{
    public BomConfigurationBomAttackDefault(){
        value = BOM_ATTACK.BOM_ATTACK_NOTHING;
    }
}
public class BomConfigurationBomAttack : BomConfigurationBase
{
    public BomConfigurationBomAttack(){
        value = BOM_ATTACK.BOM_ATTACK_THROW;
    }
}
public class BomConfigurationBomMulti : BomConfigurationBase
{
    public BomConfigurationBomMulti(){
        value = BOM_ATTACK.BOM_ATTACK_MULTI;
    }
}
