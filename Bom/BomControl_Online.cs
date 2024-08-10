using UnityEngine;
using Photon.Pun;

public class BomControl_Online : BomControl
{
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


}
