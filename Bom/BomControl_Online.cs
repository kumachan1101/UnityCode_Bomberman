using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using PlayerBomName;
using BomKind;
using BomPosName;
using BomName;

public class BomControl_Online : BomControl
{


    protected override void MakeBom_RPC(Vector3 v3, BomKind.BOM_KIND eBomKind, int iViewID, int iExplosionNum, bool bBomKick, string sMaterialType, bool bBomAttack, Vector3 direction){
        photonView.RPC(nameof(MakeBom), RpcTarget.All, v3, eBomKind,iViewID, iExplosionNum,  bBomKick, sMaterialType, bBomAttack, direction);
    }

    [PunRPC]
    public void MakeBom(Vector3 v3, BomKind.BOM_KIND eBomKind, int iViewID, int iExplosionNum, bool bBomKick, string sMaterialType, bool bBomAttack, Vector3 direction){
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

        g.transform.position = v3;
        Bom cBom = g.GetComponent<Bom>();
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
