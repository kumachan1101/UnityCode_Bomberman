using UnityEngine;
using System.Collections.Generic;

public class BomBigBan_Base : Bom_Base
{
    private HashSet<Vector2Int> processedCoordinates = new HashSet<Vector2Int>();

    protected bool XZ_Explosion(Vector3 basePosition, int x, int z)
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

        // Check if there's a wall at the explosion position
        if (IsWall(v3Temp))
        {
            return false; // Stop explosion in this direction
        }
		GameObject gExplosion = cLibrary.IsPositionAndName(v3Temp, "Explosion");
		if(null != gExplosion){
			cInsManager.DestroyInstancePool(gExplosion);
		}

        // Instantiate explosion object at the position
        GameObject g = cInsManager.InstantiateInstancePool(v3Temp);
		if(null == g){
			return false;
		}

        g.transform.position = v3Temp;
        return true;
    }

    protected override void Explosion()
    {
        if (!IsExplosion())
        {
            return;
        }

        Vector3 basePosition = cLibrary.GetPos(transform.position);
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
