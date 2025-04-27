using UnityEngine;
using Photon.Pun;

public class OnlineBomFactory : IBomFactory
{
    public Bom_Base CreateBom(GameObject gBom) => gBom.AddComponent<Bom_Online>();
    public Bom_Base CreateBomExplode(GameObject gBom) => gBom.AddComponent<BomExplode_Online>();
    public Bom_Base CreateBomBigBan(GameObject gBom) => gBom.AddComponent<BomBigBan_Online>();
}

public class BomControl_Online : BomControl
{

    protected override void InitFactory()
    {
        SetFactory(new OnlineBomFactory());
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
		Material newMaterial = materialManager.GetMaterial(bomParams.materialType);

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
				AddBomComponents(obj, bomParams.bomKind);

				Bom_Base bomBase = obj.GetComponent<Bom_Base>();
				if (bomBase != null)
				{
					bomBase.SetMaterialKind(bomParams.materialType);
					bomBase.iExplosionNum = bomParams.explosionNum;

					Bom_Base_MoveManager cMoveManager = obj.GetComponent<Bom_Base_MoveManager>();
					cMoveManager.BomAttack(bomParams);
				}
				else
				{
					Debug.LogWarning("Bom_Base component not found on the game object.");
				}
			}
		}
	}

}
