using UnityEngine;

public class Bom_CpuMpde : Bom_Base
{
     protected override bool IsExplosion(){
        if(null == cInsManager){
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
