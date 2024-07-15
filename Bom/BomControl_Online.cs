using UnityEngine;
using Photon.Pun;

public class BomControl_Online : BomControl
{

    protected override Bom_Base AddComponent_BomExplode(GameObject gBom){
        Bom_Base cBom = gBom.AddComponent<BomExplode_Online>();
        return cBom;
    }
    protected override Bom_Base AddComponent_BomBigBan(GameObject gBom){
        Bom_Base cBom = gBom.AddComponent<BomBigBan_Online>();
        return cBom;
    }
    protected override Bom_Base AddComponent_Bom(GameObject gBom){
        Bom_Base cBom = gBom.AddComponent<Bom_Online>();
        return cBom;
    }

	protected override void MakeBom_RPC(BomParameters bomParams){
		tempBom = PhotonNetwork.Instantiate("Bom", bomParams.position, Quaternion.identity);
		bomParams.viewID = tempBom.GetComponent<PhotonView>().ViewID;
		photonView.RPC(nameof(MakeBom), RpcTarget.All, bomParams);
	}


[PunRPC]
public void MakeBom(BomParameters bomParams)
{
    MaterialManager materialManager = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
    Material newMaterial = materialManager.GetMaterialOfType(bomParams.materialType);

    PhotonView targetView = PhotonView.Find(bomParams.viewID);
    if (targetView != null)
    {
        GameObject obj = targetView.gameObject;
        if (obj != null)
        {
            // 親オブジェクトと子オブジェクトのRendererコンポーネントを取得
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer == null)
            {
                renderer = obj.GetComponentInChildren<Renderer>();
            }

            if (renderer != null)
            {
                renderer.material = newMaterial;
            }
            else
            {
                Debug.LogWarning("Renderer component not found on the game object or its children.");
            }

            if (bomParams.bomKind == BOM_KIND.BOM_KIND_BIGBAN)
            {
                AddComponent_BomBigBan(obj);
            }
            else if (bomParams.bomKind == BOM_KIND.BOM_KIND_EXPLODE)
            {
                AddComponent_BomExplode(obj);
            }
            else
            {
                AddComponent_Bom(obj);
            }

            Bom_Base bomBase = obj.GetComponent<Bom_Base>();
            if (bomBase != null)
            {
                bomBase.SetMaterialKind(bomParams.materialType);
                bomBase.iExplosionNum = bomParams.explosionNum;

                if (bomParams.bomKick)
                {
                    bomBase.AbailableBomKick();
                }

                if (bomParams.bomAttack)
                {
                    bomBase.AbailableBomAttack(bomParams.direction);
                }
            }
            else
            {
                Debug.LogWarning("Bom_Base component not found on the game object.");
            }
        }
    }
}


/*
    protected override void MakeBom_RPC(Vector3 v3, BOM_KIND eBomKind, int iViewID, int iExplosionNum, bool bBomKick, string sMaterialType, bool bBomAttack, Vector3 direction){
		tempBom = PhotonNetwork.Instantiate("Bom", v3, Quaternion.identity);
        photonView.RPC(nameof(MakeBom), RpcTarget.All, v3, eBomKind, tempBom.GetComponent<PhotonView>().ViewID, iExplosionNum,  bBomKick, sMaterialType, bBomAttack, direction);
    }

    [PunRPC]
    public void MakeBom(Vector3 v3, BOM_KIND eBomKind, int iViewID, int iExplosionNum, bool bBomKick, string sMaterialType, bool bBomAttack, Vector3 direction){
        MaterialManager cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
        Material newMaterial = cMaterialMng.GetMaterialOfType(sMaterialType);

        PhotonView targetView = PhotonView.Find(iViewID);
        if (targetView != null)
        {
            GameObject obj = targetView.gameObject;
            if (obj != null)
            {
                // オブジェクトのプロパティを変更
                obj.GetComponent<Renderer>().material = newMaterial;
				if(eBomKind == BOM_KIND.BOM_KIND_BIGBAN){
					AddComponent_BomBigBan(obj);
				}
				else if(eBomKind == BOM_KIND.BOM_KIND_EXPLODE){
					AddComponent_BomExplode(obj);
				}
				else{
					AddComponent_Bom(obj);
				}
				Bom_Base cBom = obj.GetComponent<Bom_Base>();
				cBom.SetMaterialKind(sMaterialType);
				cBom.iExplosionNum = iExplosionNum;
				if(bBomKick){
					cBom.AbailableBomKick();
				}
				if(bBomAttack){
					cBom.AbailableBomAttack(direction);
				}
				tempBom = obj;
            }
        }
    }
*/
}
