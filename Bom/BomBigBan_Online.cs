using UnityEngine;

public class BomBigBan_Online : BomExplode_Online
{
    protected bool XZ_Explosion(int i, int j){
        Vector3 v3Temp = new Vector3(transform.position.x+i,transform.position.y,transform.position.z+j);
        cLibrary.DeletePositionAndName(v3Temp, "Explosion");
		GameObject g = Instantiate_Explosion(v3Temp);

        bool bRet = IsWall(v3Temp);
        if(bRet){
            Destroy(g);
            return true;
        }
        g.transform.position = v3Temp;
        return false;
    }

    protected override void Explosion()
    {
		if(false == IsExplosion()){
			Destroy(this.gameObject);
			return;
		}

        lock (lockObject)
        {
            Vector3 v3 = cLibrary.GetPos(transform.position);
            transform.position = v3;
            cLibrary.DeletePositionAndName(v3, "Explosion");
			GameObject g = Instantiate_Explosion(v3);
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

            Destroy(this.gameObject);
        }
    }

}