using UnityEngine;

public class Bom_CpuMode : Bom_Base
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
