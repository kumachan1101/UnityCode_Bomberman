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

	protected override void MakeBom_RPC(BomParameters bomParams){
		MakeBom(bomParams);
	}
    private void MakeBom(BomParameters bomParams)
    {
        GameObject g = Instantiate(BomPrefab);
        AddBomComponents(g, bomParams.bomKind);
        g.transform.position = bomParams.position;
        Bom_Base cBom = g.GetComponent<Bom_Base>();
        cBom.SetMaterialKind(bomParams.materialType);
        cBom.iExplosionNum = bomParams.explosionNum;

        Bom_Base_MoveManager cMoveManager = g.GetComponent<Bom_Base_MoveManager>();
        cMoveManager.ReqSetting(bomParams);

        tempBom = g;
    }

}
