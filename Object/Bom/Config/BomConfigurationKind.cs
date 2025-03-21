/*
public class BomConfigurationKindManager : BomConfigurationManagerBase
{
    public BomConfigurationKindManager() => configuration = new BomConfigurationKindDefalut();

    public override void Request(ReqType reqType){
        configuration = BomConfigurationFactory.Create(reqType);
    }
}
*/
public class BomConfigurationKindManager : BomConfigurationManagerBase
{
    public BomConfigurationKindManager() => configuration = BomConfigurationFactory.Create(ReqType.ExplodeBom);
}


public class BomConfigurationKindDefalut : BomConfigurationBase
{
    public BomConfigurationKindDefalut()
    {
        value = BOM_KIND.BOM_KIND_NOTHING;
    }
}


public class BomConfigurationKindExplode : BomConfigurationBase
{
    public BomConfigurationKindExplode()
    {
        value = BOM_KIND.BOM_KIND_EXPLODE;
    }
}

public class BomConfigurationKindBigBan : BomConfigurationBase
{
    public BomConfigurationKindBigBan()
    {
        value = BOM_KIND.BOM_KIND_BIGBAN;
    }
}