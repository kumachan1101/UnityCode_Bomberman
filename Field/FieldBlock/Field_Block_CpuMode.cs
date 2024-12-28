using UnityEngine;
using System.Collections.Generic;

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
        explosionManager.UpdateGroundExplosion(gObj.name, pos);
    }
    public void Rainbow(string objname)
    {
        List<GameObject> objectsToRemove = new List<GameObject>();
        List<GameObject> objectsToAdd = new List<GameObject>();

        foreach (GameObject obj in explosionManager.GetExplosionList())
        {
            if (obj == null) continue;
            if (objname != obj.name)
            {
                GameObject gobj = null;
                try
                {
                    gobj = DequeueObject(objname.Replace("(Clone)", ""));
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Failed to dequeue object: {ex.Message}");
                }

                if (gobj != null)
                {
                    objectsToAdd.Add(gobj);
                    Explosion_Base cExplosion = gobj.GetComponent<Explosion_Base>();
                    cExplosion.SetPosition(obj.transform.position);
                }
                objectsToRemove.Add(obj);
            }
        }

        explosionManager.AddToExplosionList(objectsToAdd);
        explosionManager.RemoveFromExplosionList(objectsToRemove);

        foreach (GameObject obj in objectsToRemove)
        {
            EnqueueObject(obj);
        }
    }

}