using UnityEngine;
public enum ReqType
{
    ExplodeBom,
    BigBanBom,
    FireUp,
    BomKick,
    BomAttack,
    BomUp,
    MaterialBom1,
    MaterialBom2,
    MaterialBom3,
    MaterialBom4
}

public enum GetKind
{
    BomKind,
    FireNum,
    BomKick,
    BomAttack,
    BomNum,
    MaterialType
}



public class PlayerBom
{
    private BomConfigurationManager cBomConfigManager;
    private BomListManager cBomListManager;

    public PlayerBom()
    {
        cBomConfigManager = new BomConfigurationManager();
        cBomListManager = new BomListManager();
    }

    public void Request(ReqType reqtype)
    {
        switch (reqtype)
        {
            case ReqType.ExplodeBom:
            case ReqType.BigBanBom:
            case ReqType.MaterialBom1:
            case ReqType.MaterialBom2:
            case ReqType.MaterialBom3:
            case ReqType.MaterialBom4:
                cBomConfigManager.Set(reqtype);
                break;

            case ReqType.FireUp:
            case ReqType.BomKick:
            case ReqType.BomAttack:
            case ReqType.BomUp:
                cBomConfigManager.Request(reqtype);
                break;

            default:
                break;
        }
    }

    public T Get<T>(GetKind kind)
    {
        return (T)cBomConfigManager.Get(kind);
    }

    public void Add(GameObject bom)
    {
        cBomListManager.Add(bom);
    }

    public bool IsBomAvailable(Vector3 position)
    {
        int iBomNum = (int)cBomConfigManager.Get(GetKind.BomNum);
        return cBomListManager.IsBomAvailable(position, iBomNum);
    }

    public BomParameters CreateBomParameters(Vector3 position, Vector3 direction)
    {
        BomParameters cBomParameters = new BomParameters
        {
            position = position,
            bomKind = Get<BOM_KIND>(GetKind.BomKind),
            viewID = 0,  // 必要に応じて設定
            explosionNum = Get<int>(GetKind.FireNum),
            bomKick = Get<bool>(GetKind.BomKick),
            materialType = Get<string>(GetKind.MaterialType),
            bomAttack = Get<bool>(GetKind.BomAttack),
            direction = direction
        };
        return cBomParameters;
    }
}
