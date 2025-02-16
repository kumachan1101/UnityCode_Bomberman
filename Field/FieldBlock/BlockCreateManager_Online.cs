using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class BlockCreateManager_Online : BlockCreateManager {

    void Start()
    {
        cLibrary = GameObject.Find("Library").GetComponent<Library_Base>();
		CreateFixedBlock();
    }

    protected override void CreateAndInitializeBrokenBlockManager()
    {
        brokenBlockManager = CreateBlockManager<BrokenBlockManager_Online>();
        brokenBlockManager?.Initialize();
    }

	protected override void SetFieldRange(){
		GameManager.SetFieldRange(10,10);
	}

    protected override void InsObjMove_RPC(int x, int y, int z, Library_Base.Direction randomDirection){
		if(false == GetComponent<PhotonView>().IsMine){
			return;
		}
        photonView.RPC(nameof(InsObjMove), RpcTarget.All, x, y, z, randomDirection);
    }

    protected override ExplosionManager CreateExplosionManager()
    {
        GameObject gExplosionManager = GameObject.Find("ExplosionManager");
        var manager = gExplosionManager.AddComponent<ExplosionManager_Online>();

        PoolerType type = (GetComponent<BlockCreateManager_Tower>() != null) ? PoolerType.Tower : PoolerType.Local;
        manager.Initialize(type);

        return manager;
    }

}

public class BrokenBlockManager_Online : BrokenBlockManager
{
    protected override void InsBrokenBlock_RPC(int x, int y, int z){
        photonView.RPC(nameof(InsBrokenBlock), RpcTarget.All, x, y, z);
    }
 

}