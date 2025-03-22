using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

public class BlockCreateManager_CpuMode : BlockCreateManager {


    void Start()
    {
        CreateFixedBlock();
        CreateBrokenBlock();
        CompleteBlockCreate();
    }


    protected override void CreateAndInitializeBrokenBlockManager()
    {
        brokenBlockManager = CreateBlockManager<BrokenBlockManager_CpuMode>();
        brokenBlockManager?.Initialize();
    }

    protected override void InsObjMove_RPC(int x, int y, int z, Library_Base.Direction randomDirection){
        InsObjMove(x, y, z, randomDirection);
    }

    protected override void CreateExplosionManager()
    {
        GameObject gExplosionManager = GameObject.Find("ExplosionManager");
        var manager = gExplosionManager.AddComponent<ExplosionManager_CpuMode>();

        PoolerType type = (GetComponent<TowerSpawnManager>() != null) ? PoolerType.Tower : PoolerType.Local;
        manager.Initialize(type);

        return;
    }
}

public class BrokenBlockManager_CpuMode : BrokenBlockManager
{
    protected override void InsBrokenBlock_RPC(int x, int y, int z){
        InsBrokenBlock(x, y, z);
    }
}