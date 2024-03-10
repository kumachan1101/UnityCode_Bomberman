using UnityEngine;
using PlayerBomName;
using BomKind;
using BomPosName;
using BomName;

public class BomControl_CpuMode : BomControl
{
    public override void Awake(){
        cItemControl = GameObject.Find("ItemControl").GetComponent<ItemControl_CpuMode>();
        cField = GameObject.Find("Field").GetComponent<Field_CpuMode>();
    }

    protected override void MakeBom_RPC(Vector3 v3, BomKind.BOM_KIND eBomKind, int iViewID, int iExplosionNum, bool bBomKick, string sMaterialType){
        MakeBom_CPU(v3, eBomKind,iViewID, iExplosionNum,  bBomKick, sMaterialType);
    }

    private void MakeBom_CPU(Vector3 v3, BomKind.BOM_KIND eBomKind, int iViewID, int iExplosionNum, bool bBomKick, string sMaterialType){
        GameObject g;
        if(eBomKind == BomKind.BOM_KIND.BOM_KIND_BIGBAN){
            g = Instantiate(BomBigBanPrefab);    
        }
        else if(eBomKind == BomKind.BOM_KIND.BOM_KIND_EXPLODE){
            g = Instantiate(BomExplodePrefab);
        }
        else{
            g = Instantiate(BomPrefab);
        }

        MaterialManager cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
        Material newMaterial = cMaterialMng.GetMaterialOfType(sMaterialType);
        g.GetComponent<Renderer>().material = newMaterial;
        g.GetComponent<Bom>().SetMaterialType(newMaterial);
        //g.GetComponent<Bom>().ExplosionPrefab.GetComponent<Renderer>().material = newMaterial;

        g.transform.position = v3;
        Bom cBom = g.GetComponent<Bom>();
        cBom.iExplosionNum = iExplosionNum;
        cBom.SetViewID(iViewID);
        if(bBomKick){
            cBom.AbailableBomKick();
        }
        tempBom = g;
    }

}
