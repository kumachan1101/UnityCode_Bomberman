
public class BomConfigurationManager
{
    private BomConfigurationBase cExplosionNum;
    private BomConfigurationBase cMaterialType;
    private BomConfigurationBase cBomkind;
    private BomConfigurationBase cBomAttack;
    private BomConfigurationBase cBomKick;
    private BomConfigurationBase cBomUp;

    public BomConfigurationManager(){
        cExplosionNum = new BomConfigurationFireUp();
        cMaterialType = new BomConfigurationMaterialType();
        cBomkind = new BomConfigurationKind();
        cBomAttack = new BomConfigurationBomAttack();
        cBomKick = new BomConfigurationBomKick();
        cBomUp = new BomConfigurationBomUp();
    }

    public void Request(ReqType reqtype)
    {
        BomConfigurationBase bomConfiguration = null;
        switch (reqtype)
        {
            case ReqType.FireUp:
                bomConfiguration = cExplosionNum;
                break;

            case ReqType.BomAttack:
                bomConfiguration = cBomAttack;
                break;

            case ReqType.BomKick:
                bomConfiguration = cBomKick;
                break;

            case ReqType.BomUp:
                bomConfiguration = cBomUp;
                break;

            default:
                break;
        }
        bomConfiguration.Request();
    }

    public object Get(GetKind getKind)
    {
        BomConfigurationBase bomConfiguration = null;
        switch (getKind)
        {
            case GetKind.BomKind:
                bomConfiguration = cBomkind;
                break;

            case GetKind.FireNum:
                bomConfiguration = cExplosionNum;
                break;

            case GetKind.MaterialType:
                bomConfiguration = cMaterialType;
                break;

            case GetKind.BomAttack:
                bomConfiguration = cBomAttack;
                break;

            case GetKind.BomKick:
                bomConfiguration = cBomKick;
                break;

            case GetKind.BomNum:
                bomConfiguration = cBomUp;
                break;

            default:
                // デフォルトの処理（エラーハンドリングなど）
                break;
        }
        return bomConfiguration.Get();
    }

    public void Set(ReqType configType){
        switch (configType)
        {
            case ReqType.ExplodeBom:
            case ReqType.BigBanBom:
                cBomkind.Set(ConvertToBomKind(configType));
                break;

            case ReqType.MaterialBom1:
            case ReqType.MaterialBom2:
            case ReqType.MaterialBom3:
            case ReqType.MaterialBom4:
                cMaterialType.Set(ConvertToMaterialType(configType));
                break;

            default:
                break;
        }
    }

    public string ConvertToMaterialType(ReqType reqType)
    {
        string result;

        switch (reqType)
        {
            case ReqType.MaterialBom1:
                result = MaterialTypes.BomMaterial1;
                break;

            case ReqType.MaterialBom2:
                result = MaterialTypes.BomMaterial2;
                break;

            case ReqType.MaterialBom3:
                result = MaterialTypes.BomMaterial3;
                break;

            case ReqType.MaterialBom4:
                result = MaterialTypes.BomMaterial4;
                break;

            default:
                result = MaterialTypes.BomMaterial1; // デフォルト値
                break;
        }

        return result;
    }

    public BOM_KIND ConvertToBomKind(ReqType reqType)
    {
        BOM_KIND result;

        switch (reqType)
        {
            case ReqType.ExplodeBom:
                result = BOM_KIND.BOM_KIND_EXPLODE;
                break;

            case ReqType.BigBanBom:
                result = BOM_KIND.BOM_KIND_BIGBAN;
                break;

            default:
                result = BOM_KIND.BOM_KIND_NOTHING; // デフォルト値
                break;
        }

        return result;
    }


}
