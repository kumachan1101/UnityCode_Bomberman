using UnityEngine;

public class CpuModeBomFactory : IBomFactory
{
    public Bom_Base CreateBom(GameObject gBom) => gBom.AddComponent<Bom_CpuMode>();
    public Bom_Base CreateBomExplode(GameObject gBom) => gBom.AddComponent<BomExplode_CpuMode>();
    public Bom_Base CreateBomBigBan(GameObject gBom) => gBom.AddComponent<BomBigBan_CpuMode>();
}

public class BomControl_CpuMode : BomControl
{
    protected override void InitFactory()
    {
        SetFactory(new CpuModeBomFactory());
    }
/*
    protected override Bom_Base AddComponent_BomExplode(GameObject gBom){
        Bom_Base cBom = gBom.AddComponent<BomExplode_CpuMode>();
        return cBom;
    }
    protected override Bom_Base AddComponent_BomBigBan(GameObject gBom){
        Bom_Base cBom = gBom.AddComponent<BomBigBan_CpuMode>();
        return cBom;
    }
    protected override Bom_Base AddComponent_Bom(GameObject gBom){
        Bom_Base cBom = gBom.AddComponent<Bom_CpuMode>();
        return cBom;
    }
*/
	protected override void MakeBom_RPC(BomParameters bomParams){
		MakeBom(bomParams);
	}
    private void MakeBom(BomParameters bomParams)
    {
        GameObject g = Instantiate(BomPrefab);
        AddBomComponents(g, bomParams.bomKind);
        //DeleteComponent_Bom(g); // 必要なら削除コンポーネントの処理を追加
        /*
        if (bomParams.bomKind == BOM_KIND.BOM_KIND_BIGBAN)
        {
            AddComponent_BomBigBan(g);
        }
        else if (bomParams.bomKind == BOM_KIND.BOM_KIND_EXPLODE)
        {
            AddComponent_BomExplode(g);
        }
        else
        {
            AddComponent_Bom(g);
        }
        */

        g.transform.position = bomParams.position;
        Bom_Base cBom = g.GetComponent<Bom_Base>();
        cBom.SetMaterialKind(bomParams.materialType);
        cBom.iExplosionNum = bomParams.explosionNum;

        Bom_Base_MoveManager cMoveManager = g.GetComponent<Bom_Base_MoveManager>();
        cMoveManager.ReqSetting(bomParams);

        tempBom = g;
    }

}
