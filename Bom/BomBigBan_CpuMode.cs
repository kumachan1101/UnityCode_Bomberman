using UnityEngine;

public class BomBigBan_CpuMode : BomBigBan_Base
{
	protected override void init(){
		cInsManager = gameObject.AddComponent<InstanceManager_CpuMode>();
		sExplosion = cMaterialMng.GetMaterialOfExplosion(sMaterialKind);
		base.init();
	}

	protected override bool IsExplosion(){
		return true;
	}

/*
    protected bool XZ_Explosion(int i, int j)
    {
        Vector3 v3Temp = new Vector3(transform.position.x + i, transform.position.y, transform.position.z + j);
        bool isWall = IsWall(v3Temp);
        if (isWall)
        {
            return true;
        }
		GameObject gExplosion = cLibrary.IsPositionAndName(v3Temp, "Explosion");
		if(null != gExplosion){
			cInsManager.DestroyInstance(gExplosion);	
		}

        GameObject explosion = cInsManager.InstantiateInstance(v3Temp);
        return false;
    }

    protected override void Explosion()
    {
        if (!IsExplosion())
        {
            return;
        }

        lock (lockObject)
        {
            Vector3 position = cLibrary.GetPos(transform.position);
            transform.position = position;
			GameObject gExplosion = cLibrary.IsPositionAndName(position, "Explosion");
			if(null != gExplosion){
				cInsManager.DestroyInstance(gExplosion);	
			}

            cInsManager.InstantiateInstance(position);

            // 上下左右の爆発処理
            for (int j = 1; j <= iExplosionNum; j++)
            {
                if (XZ_Explosion(0, j)) break;
            }
            for (int j = 1; j <= iExplosionNum; j++)
            {
                if (XZ_Explosion(0, -j)) break;
            }
            for (int i = 1; i <= iExplosionNum; i++)
            {
                if (XZ_Explosion(-i, 0)) break;

                for (int j = 1; j <= iExplosionNum; j++)
                {
                    if (XZ_Explosion(-i, j)) break;
                }
                for (int j = 1; j <= iExplosionNum; j++)
                {
                    if (XZ_Explosion(-i, -j)) break;
                }
            }
            for (int i = 1; i <= iExplosionNum; i++)
            {
                if (XZ_Explosion(i, 0)) break;

                for (int j = 1; j <= iExplosionNum; j++)
                {
                    if (XZ_Explosion(i, j)) break;
                }
                for (int j = 1; j <= iExplosionNum; j++)
                {
                    if (XZ_Explosion(i, -j)) break;
                }
            }

            cInsManager.DestroyInstance(this.gameObject);
        }
    }
}
*/
/*
using UnityEngine;

public class BomBigBan_CpuMode : BomExplode_CpuMode
{
    protected bool XZ_Explosion(int i, int j){
        Vector3 v3Temp = new Vector3(transform.position.x+i,transform.position.y,transform.position.z+j);
        bool bRet = IsWall(v3Temp);
        if(bRet){
            return true;
        }
        cLibrary.IsPositionAndName(v3Temp, "Explosion");
        GameObject g = Instantiate(ExplosionPrefab);
        g.transform.position = v3Temp;
        return false;
    }

    protected override void Explosion()
    {
		if(false == IsExplosion()){
			return;
		}

        lock (lockObject)
        {
            Vector3 v3 = cLibrary.GetPos(transform.position);
            transform.position = v3;
            cLibrary.IsPositionAndName(v3, "Explosion");
            GameObject g = Instantiate(ExplosionPrefab);
            g.transform.position = v3;
			bool bFlag1_1 = false;
			bool bFlag1_2 = false;
			bool bFlag1_3 = false;
			bool bFlag2_1 = false;
			bool bFlag2_2 = false;
			bool bFlag2_3 = false;

            for (int j = 1; j <= iExplosionNum; j++) 
            {
                if(XZ_Explosion(0, j)){
                    break;
                }
            }
            for (int j = 1; j <= iExplosionNum; j++) 
            {
                if(XZ_Explosion(0, j*(-1))){
                    break;
                }
            }
            for (int i = 1; i <= iExplosionNum; i++) 
            {
                if(XZ_Explosion(i*(-1), 0)){
					bFlag1_1 = true;
                    break;
                }
				if(bFlag1_1){
					break;
				}
                for (int j = 1; j <= iExplosionNum; j++) 
                {
					if(bFlag1_2){
						break;
					}
                    if(XZ_Explosion(i*(-1), j)){
						bFlag1_2 = true;
                        break;
                    }
                }
                for (int j = 1; j <= iExplosionNum; j++) 
                {
					if(bFlag1_3){
						break;
					}
                    if(XZ_Explosion(i*(-1), j*(-1))){
						bFlag1_3 = true;
                        break;
                    }
                }
            }

            for (int i = 1; i <= iExplosionNum; i++) 
            {
                if(XZ_Explosion(i, 0)){
					bFlag2_1 = true;
	                break;
                }
				if(bFlag2_1){
					break;
				}
                for (int j = 1; j <= iExplosionNum; j++) 
                {
					if(bFlag2_2){
						break;
					}
                    if(XZ_Explosion(i, j)){
						bFlag2_2 = true;
                        break;
                    }
                }
                for (int j = 1; j <= iExplosionNum; j++) 
                {
					if(bFlag2_3){
						break;
					}
                    if(XZ_Explosion(i, j*(-1))){
						bFlag2_3 = true;
                        break;
                    }
                }
            }
            DestroySync(this.gameObject);
        }
    }
*/
}
