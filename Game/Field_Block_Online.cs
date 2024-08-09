using UnityEngine;
using Photon.Pun;
using System.Reflection;

public class Field_Block_Online : Field_Block_Base {

    void Start()
    {
		objectPooler = GetComponent<ObjectPooler_Base>();
		//SetupStage();

        cLibrary = GameObject.Find("Library").GetComponent<Library_Base>();
		CreateFixedBlock();
    }

/*
	protected override void DestroySync(GameObject g){
		PhotonView pv = g.GetComponent<PhotonView>();
		if (pv != null && pv.IsMine)
		{
			PhotonNetwork.Destroy(pv.gameObject); // pvではなくpv.gameObjectを渡す
		}
	}
*/

	protected override void SetFieldRange(){
		GameManager.SetFieldRange(10,10);
	}
    protected override void ClearBrokenList_RPC(){
        photonView.RPC(nameof(ClearBrokenList), RpcTarget.All);
    }

    protected override void InsBrokenBlock_RPC(int x, int y, int z){
        photonView.RPC(nameof(InsBrokenBlock), RpcTarget.All, x, y, z);
    }


    protected override void InsObjMove_RPC(int x, int y, int z, Library_Base.Direction randomDirection){
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}
        photonView.RPC(nameof(InsObjMove), RpcTarget.All, x, y, z, randomDirection);
    }

    protected override void Rainbow_RPC(string sMaterialType){
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}
        photonView.RPC(nameof(Rainbow), RpcTarget.All, sMaterialType);
    }

    public override string GetExplosionType(string input)
    {
        if (input.Contains(ExplosionOnlineTypes.Explosion1))
        {
            return ExplosionOnlineTypes.Explosion1;
        }
        else if (input.Contains(ExplosionOnlineTypes.Explosion2))
        {
            return ExplosionOnlineTypes.Explosion2;
        }
        else if (input.Contains(ExplosionOnlineTypes.Explosion3))
        {
            return ExplosionOnlineTypes.Explosion3;
        }
        else if (input.Contains(ExplosionOnlineTypes.Explosion4))
        {
            return ExplosionOnlineTypes.Explosion4;
        }
        else
        {
            return null;
        }
    }

}