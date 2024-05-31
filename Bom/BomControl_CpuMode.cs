using UnityEngine;
using PlayerBomName;
using BomKind;
using BomPosName;

public class BomControl_CpuMode : BomControl
{

    protected override Bom_Base AddComponent_Bom(GameObject gBom){
        Bom_Base cBom = gBom.AddComponent<Bom_CpuMode>();
        return cBom;
    }
    protected override Bom_Base AddComponent_BomExplode(GameObject gBom){
        Bom_Base cBom = gBom.AddComponent<BomExplode_CpuMode>();
        return cBom;
    }
    protected override Bom_Base AddComponent_BomBigBan(GameObject gBom){
        Bom_Base cBom = gBom.AddComponent<BomBigBan_CpuMode>();
        return cBom;
    }



    protected override void MakeBom_RPC(Vector3 v3, BomKind.BOM_KIND eBomKind, int iViewID, int iExplosionNum, bool bBomKick, string sMaterialType, bool bBomAttack, Vector3 direction){
        MakeBom(v3, eBomKind,iViewID, iExplosionNum,  bBomKick, sMaterialType, bBomAttack, direction);
    }

    private void MakeBom(Vector3 v3, BomKind.BOM_KIND eBomKind, int iViewID, int iExplosionNum, bool bBomKick, string sMaterialType, bool bBomAttack, Vector3 direction){
        GameObject g = Instantiate(BomPrefab);
		//DeleteComponent_Bom(g);
        if(eBomKind == BomKind.BOM_KIND.BOM_KIND_BIGBAN){
			AddComponent_BomBigBan(g);
        }
        else if(eBomKind == BomKind.BOM_KIND.BOM_KIND_EXPLODE){
			AddComponent_BomExplode(g);
        }
        else{
			AddComponent_Bom(g);
        }

        //MaterialManager cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
        //Material newMaterial = cMaterialMng.GetMaterialOfType(sMaterialType);
        //g.GetComponent<Renderer>().material = newMaterial;
        //g.GetComponent<Bom_Base>().SetMaterialType(newMaterial);

        g.transform.position = v3;
        Bom_Base cBom = g.GetComponent<Bom_Base>();
		cBom.SetMaterialKind(sMaterialType);
        cBom.iExplosionNum = iExplosionNum;
        cBom.SetViewID(iViewID);
        if(bBomKick){
            cBom.AbailableBomKick();
        }
        if(bBomAttack){
            cBom.AbailableBomAttack(direction);
        }

        tempBom = g;
    }

}
