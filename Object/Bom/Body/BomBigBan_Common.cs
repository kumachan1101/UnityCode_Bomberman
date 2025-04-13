using UnityEngine;
using System.Collections.Generic;

public class BomBigBan_Common : MonoBehaviour 
{
    private InstanceManager_Base cInsManager;
    protected HashSet<Vector2Int> processedCoordinates = new HashSet<Vector2Int>();

    public void SetInstanceManager(InstanceManager_Base cManager){
        cInsManager = cManager;
    }
/*
    public void BomBiBomBigBan_Explosion(Vector3 basePosition)
    {
        processedCoordinates.Clear();
        int iExplosionNum = 2;

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
*/

    public void BomBiBomBigBan_Explosion(Vector3 basePosition)
    {
        processedCoordinates.Clear();
        int iExplosionNum = 2;

        // x, z にそれぞれ ±1 の組み合わせを渡す
        int[] directions = { 1, -1 };

        foreach (int xSign in directions)
        {
            foreach (int zSign in directions)
            {
                for (int i = 0; i <= iExplosionNum; i++)
                {
                    for (int j = 0; j <= iExplosionNum; j++)
                    {
                        XZ_Explosion(basePosition, i * xSign, j * zSign);
                    }
                }
            }
        }

        // Destroy the bomb object after explosion
        cInsManager.DestroyInstance(this.gameObject);
    }

    private bool XZ_Explosion(Vector3 basePosition, int x, int z)
    {

        Vector2Int coord = new Vector2Int(x, z);
        // Check if this coordinate has already been processed
        if (processedCoordinates.Contains(coord))
        {
            return false; // Skip processing
        }
        // Mark this coordinate as processed
        processedCoordinates.Add(coord);

        //Debug.Log("x:" + x + " " + "y:" + z);
        Vector3 v3Temp = new Vector3(basePosition.x + x, basePosition.y, basePosition.z + z);
		if(GameManager.xmax <= v3Temp.x || GameManager.zmax <= v3Temp.z || 0 > v3Temp.x || 0 > v3Temp.z){
			return false;
		}

        if(DestroyExistingExplosion(v3Temp)){
            cInsManager.InstantiateInstancePool(v3Temp);
        }
        
        return true;
    }
    protected bool DestroyExistingExplosion(Vector3 position)
    {
        GameObject existingExplosion = Library_Base.GetGameObjectAtExactPositionWithName(position, "Explosion");
        if (existingExplosion != null)
        {
            cInsManager.DestroyInstancePool(existingExplosion);
        }
        return true;
    }

}
