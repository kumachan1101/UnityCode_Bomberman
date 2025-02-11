using UnityEngine;
using System.Collections.Generic;

public class Field_Block_CpuMode : Field_Block_Base {

    protected override void InsBrokenBlock_RPC(int x, int y, int z){
        InsBrokenBlock(x, y, z);
    }


    protected override void InsObjMove_RPC(int x, int y, int z, Library_Base.Direction randomDirection){
        InsObjMove(x, y, z, randomDirection);
    }

    public override void Rainbow_RPC(string sMaterialType){
        Rainbow(sMaterialType);
    }
    public override void UpdateGroundExplosion(GameObject gObj)
    {
        Vector3 pos = gObj.transform.position;
        explosionManager.EnqueueObject(gObj);
        explosionManager.UpdateGroundExplosion(gObj.name, pos);
    }
    public void Rainbow(string objname){
        explosionManager.Rainbow(objname);
    }

}