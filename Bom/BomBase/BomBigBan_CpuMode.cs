using UnityEngine;

public class BomBigBan_CpuMode : BomBigBan_Base
{
     protected override bool IsExplosion(){
        if(null == cInsManager){
            return false;
        }
        return true;
    }

    protected override void Explosion()
    {
        if (!IsExplosion())
        {
            return;
        }

        Vector3 basePosition = Library_Base.GetPos(transform.position);
        transform.position = basePosition;

        // Reset processed coordinates
        processedCoordinates.Clear();

        // Explode in X and Z directions (positive and negative)

        for (int i = 0; i <= iExplosionNum; i++)
        {
            for (int j = 0; j <= iExplosionNum; j++)
            {
				XZ_Explosion(basePosition, i, j);
            }
        }
        for (int i = 0; i <= iExplosionNum; i++)
        {
            for (int j = 0; j <= iExplosionNum; j++)
            {
                XZ_Explosion(basePosition, i, -j);
            }
        }
        for (int i = 0; i <= iExplosionNum; i++)
        {
            for (int j = 0; j <= iExplosionNum; j++)
            {
				XZ_Explosion(basePosition, -i, j);
            }
        }
        for (int i = 0; i <= iExplosionNum; i++)
        {
            for (int j = 0; j <= iExplosionNum; j++)
            {
                XZ_Explosion(basePosition, -i, -j);
            }
        }

        // Destroy the bomb object after explosion
        cInsManager.DestroyInstance(this.gameObject);
    }
}
