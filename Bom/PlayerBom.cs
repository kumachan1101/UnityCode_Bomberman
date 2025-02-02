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



public class PlayerBom : MonoBehaviour
{
    private BomConfigurationManager cBomConfigManager;
    private BomListManager cBomListManager;
    private MaterialManager materialManager;

    public void Awake()
    {
        cBomConfigManager = new BomConfigurationManager();
        cBomListManager = new BomListManager();
        materialManager = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
		string MaterialType = materialManager.GetBomMaterialByPlayerName(this.gameObject.name);
        Request(materialManager.GetReqtypeOfMaterial(MaterialType));

    }

    public void Request(ReqType reqtype)
    {
        cBomConfigManager.Request(reqtype);
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
