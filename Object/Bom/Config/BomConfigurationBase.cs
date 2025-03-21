
public abstract class BomConfigurationManagerBase
{
    protected BomConfigurationBase configuration;
    //public virtual void Request(ReqType reqType){}
    public virtual void Request() {
        configuration.Request();
    }
    public virtual void Set(ReqType reqType) {
        configuration = BomConfigurationFactory.Create(reqType);
        configuration.Request();
    }
    public object Get() => configuration.Get();
}
public static class BomConfigurationFactory
{
    public static BomConfigurationBase Create(ReqType reqType)
    {
        return reqType switch
        {
            ReqType.FireUp => new BomConfigurationFireUp(),
            ReqType.MaterialBom1 => new BomConfigurationMaterialType1(),
            ReqType.MaterialBom2 => new BomConfigurationMaterialType2(),
            ReqType.MaterialBom3 => new BomConfigurationMaterialType3(),
            ReqType.MaterialBom4 => new BomConfigurationMaterialType4(),
            ReqType.ExplodeBom => new BomConfigurationKindExplode(),
            ReqType.BigBanBom => new BomConfigurationKindBigBan(),
            ReqType.BomAttack => new BomConfigurationBomAttack(),
            ReqType.BomKick => new BomConfigurationBomKick(),
            ReqType.BomUp => new BomConfigurationBomUp(),
            _ => null
        };
    }
}

public class BomConfigurationBase
{
    protected object value;

    public virtual void Request() { }

    public object Get()
    {
        return value;
    }
}