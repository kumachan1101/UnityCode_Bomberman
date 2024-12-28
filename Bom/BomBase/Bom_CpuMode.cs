using UnityEngine;

public class Bom_CpuMode : Bom_Base
{
    override protected void AddComponentInstanceManager(){
        cInsManager = gameObject.AddComponent<InstanceManager_CpuMode>();
    }
 
     protected override bool IsExplosion(){
        if(null == cInsManager){
            return false;
        }
        Vector3 v3 = Library_Base.GetPos(transform.position);
        if (Library_Base.IsPositionOutOfBounds(v3)){
            return false;
        }
        return true;
    }

    protected override void Explosion()
    {
        if (!IsExplosion()) return;

        Vector3 v3 = Library_Base.GetPos(transform.position);
        HandleExplosion(v3);
    }
}
