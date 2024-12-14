using UnityEngine;
public class Field_Block_CpuMode : Field_Block_Base {

    protected override void ClearBrokenList_RPC(){
        ClearBrokenList();
    }


    protected override void InsBrokenBlock_RPC(int x, int y, int z){
        InsBrokenBlock(x, y, z);
    }


    protected override void InsObjMove_RPC(int x, int y, int z, Library_Base.Direction randomDirection){
        InsObjMove(x, y, z, randomDirection);
    }


    public override void Rainbow_RPC(string sMaterialType){
        Rainbow(sMaterialType);
    }

    public override GameObject DequeueObject(string tag)
    {
        return explosionManager.DequeueObject(tag);
    }

    public override void EnqueueObject(GameObject obj)
    {
		explosionManager.EnqueueObject(obj);
    }
    public override void UpdateGroundExplosion(GameObject gObj)
    {
        Vector3 pos = gObj.transform.position;
        EnqueueObject(gObj);
        //explosionManager.ExplosionList_temp.Add(gObj);
        explosionManager.UpdateGroundExplosion(gObj.name, pos);
        //explosionManager.UpdateGroundExplosion(gObj);
    }

}