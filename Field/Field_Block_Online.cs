using UnityEngine;
using Photon.Pun;
using System.Reflection;

public class Field_Block_Online : Field_Block_Base {

    void Start()
    {
		objectPooler = GetComponent<ObjectPooler_Base>();
        cLibrary = GameObject.Find("Library").GetComponent<Library_Base>();
		CreateFixedBlock();
    }


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

    public override void Rainbow_RPC(string sMaterialType){
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}
        photonView.RPC(nameof(Rainbow), RpcTarget.All, sMaterialType);
    }



}