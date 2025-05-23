/*

public class BomConfigurationManager
{
    private BomConfigurationFireUpManager cExplosionNumManager;
    private BomConfigurationMaterialTypeManager cMaterialTypeManager;
    private BomConfigurationKindManager cBomkindManager;
    private BomConfigurationBomAttackManager cBomAttackManager;
    private BomConfigurationBomKickManager cBomKickManager;
    private BomConfigurationBomUpManager cBomUpManager;

    public BomConfigurationManager()
    {
        cExplosionNumManager = new BomConfigurationFireUpManager();
        cMaterialTypeManager = new BomConfigurationMaterialTypeManager();
        cBomkindManager = new BomConfigurationKindManager();
        cBomAttackManager = new BomConfigurationBomAttackManager();
        cBomKickManager = new BomConfigurationBomKickManager();
        cBomUpManager = new BomConfigurationBomUpManager();
    }

    public void Request(ReqType reqtype)
    {
        switch (reqtype)
        {
            case ReqType.FireUp:
                cExplosionNumManager.Request(reqtype);
                break;
            case ReqType.MaterialBom1:
            case ReqType.MaterialBom2:
            case ReqType.MaterialBom3:
            case ReqType.MaterialBom4:
                cMaterialTypeManager.Request(reqtype);
                break;
            case ReqType.ExplodeBom:
            case ReqType.BigBanBom:
                cBomkindManager.Request(reqtype);
                break;
            case ReqType.BomAttack:
                cBomAttackManager.Request(reqtype);
                break;
            case ReqType.BomKick:
                cBomKickManager.Request(reqtype);
                break;
            case ReqType.BomUp:
                cBomUpManager.Request(reqtype);
                break;
            default:
                break;
        }
    }

    public object Get(GetKind getKind)
    {
        return getKind switch
        {
            GetKind.BomKind => cBomkindManager.Get(),
            GetKind.FireNum => cExplosionNumManager.Get(),
            GetKind.MaterialType => cMaterialTypeManager.Get(),
            GetKind.BomAttack => cBomAttackManager.Get(),
            GetKind.BomKick => cBomKickManager.Get(),
            GetKind.BomNum => cBomUpManager.Get(),
            _ => null
        };
    }
}
*/
public class BomConfigurationManager
{
    private BomConfigurationManagerBase cExplosionNumManager;
    private BomConfigurationManagerBase cMaterialTypeManager;
    private BomConfigurationManagerBase cBomkindManager;
    private BomConfigurationManagerBase cBomAttackManager;
    private BomConfigurationManagerBase cBomKickManager;
    private BomConfigurationManagerBase cBomUpManager;
    
    public BomConfigurationManager()
    {
        cExplosionNumManager = new BomConfigurationFireUpManager();
        cMaterialTypeManager = new BomConfigurationMaterialTypeManager();
        cBomkindManager = new BomConfigurationKindManager();
        cBomAttackManager = new BomConfigurationBomAttackManager();
        cBomKickManager = new BomConfigurationBomKickManager();
        cBomUpManager = new BomConfigurationBomUpManager();
    }
    public void Request(ReqType reqType)
    {
        switch (reqType)
        {
            case ReqType.FireUp:
                cExplosionNumManager.Request();
                break;
            case ReqType.MaterialBom1:
            case ReqType.MaterialBom2:
            case ReqType.MaterialBom3:
            case ReqType.MaterialBom4:
                cMaterialTypeManager.Set(reqType);
                break;
            case ReqType.ExplodeBom:
            case ReqType.BigBanBom:
                cBomkindManager.Set(reqType);
                break;
            case ReqType.BomAttack:
            case ReqType.BomMulti:
                cBomAttackManager.Set(reqType);
                break;
            case ReqType.BomKick:
                cBomKickManager.Request();
                break;
            case ReqType.BomUp:
                cBomUpManager.Request();
                break;
            default:
                break;
        }
    }
    public object Get(GetKind getKind)
    {
        return getKind switch
        {
            GetKind.BomKind => cBomkindManager.Get(),
            GetKind.FireNum => cExplosionNumManager.Get(),
            GetKind.MaterialType => cMaterialTypeManager.Get(),
            GetKind.BomAttack => cBomAttackManager.Get(),
            GetKind.BomKick => cBomKickManager.Get(),
            GetKind.BomNum => cBomUpManager.Get(),
            _ => null
        };
    }
}