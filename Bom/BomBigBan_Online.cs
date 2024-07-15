using Photon.Pun;
public class BomBigBan_Online : BomBigBan_Base
{
	protected override void init(){
		cInsManager = gameObject.AddComponent<InstanceManager_Online>();
        cInsManager.SetResource(sExplosion);
	}

	protected override bool IsExplosion(){
		return GetComponent<PhotonView>().IsMine;
	}
/*
    protected bool XZ_Explosion(int i, int j)
	{
        Vector3 v3Temp = new Vector3(transform.position.x+i,transform.position.y,transform.position.z+j);
        bool bRet = IsWall(v3Temp);
        if(bRet){
            return true;
        }

		GameObject gExplosion = cLibrary.IsPositionAndName(v3Temp, "Explosion");
		if(null != gExplosion){
			cInsManager.DestroyInstance(gExplosion);	
		}

		GameObject g = cInsManager.InstantiateInstance(v3Temp);
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
			GameObject gExplosion = cLibrary.IsPositionAndName(v3, "Explosion");
			if(null != gExplosion){
				cInsManager.DestroyInstance(gExplosion);	
			}

			GameObject g = cInsManager.InstantiateInstance(v3);
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

            cInsManager.DestroyInstance(this.gameObject);
        }
    }
*/
}