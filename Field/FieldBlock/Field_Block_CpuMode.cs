using UnityEngine;
using System.Collections.Generic;

public class Field_Block_CpuMode : Field_Block_Base {

    protected override void CreateAndInitializeBrokenBlockManager()
    {
        brokenBlockManager = CreateBlockManager<BrokenBlockManager_CpuMode>();
        brokenBlockManager?.Initialize();
    }

    protected override void InsObjMove_RPC(int x, int y, int z, Library_Base.Direction randomDirection){
        InsObjMove(x, y, z, randomDirection);
    }

    protected override ExplosionManager CreateExplosionManager()
    {
        //var obj = new GameObject("ExplosionManager");
        
        GameObject gExplosionManager = GameObject.Find("ExplosionManager");
        var manager = gExplosionManager.AddComponent<ExplosionManager_CpuMode>();

        PoolerType type = (GetComponent<Field_Block_Tower>() != null) ? PoolerType.Tower : PoolerType.Local;
        manager.Initialize(type);

        return manager;
    }

}

public class BrokenBlockManager_CpuMode : BrokenBlockManager
{
    protected override void InsBrokenBlock_RPC(int x, int y, int z){
        InsBrokenBlock(x, y, z);
    }





}