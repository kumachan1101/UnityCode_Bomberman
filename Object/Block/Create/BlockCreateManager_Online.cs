using UnityEngine;
using Photon.Pun;

public class BlockCreateManager_Online : BlockCreateManager {

    void Start()
    {
		CreateFixedBlock();
    }

    protected override void CreateBrokenBlockManager()
    {
        brokenBlockManager = CreateBlockManager<BrokenBlockManager_Online>();
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

    protected override void CreateExplosionManager()
    {
        GameObject gExplosionManager = GameObject.Find("ExplosionManager");
        var manager = gExplosionManager.AddComponent<ExplosionManager_Online>();

        PoolerType type = (GetComponent<TowerSpawnManager>() != null) ? PoolerType.Tower : PoolerType.Local;
        manager.Initialize(type);

        return;
    }

}

public class BrokenBlockManager_Online : BrokenBlockManager
{
    protected override void InsBrokenBlock_RPC(int x, int y, int z){
        photonView.RPC(nameof(InsBrokenBlock), RpcTarget.All, x, y, z);
    }
 

}