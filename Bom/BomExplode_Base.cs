using UnityEngine;

public class BomExplode_Base : Bom_Base
{
	
	protected override bool XorZ_Explosion(Vector3 v3Temp){
		bool bRet = IsWall(v3Temp);
		if(bRet){
			return false;
		}
		GameObject gExplosion = cLibrary.IsPositionAndName(v3Temp, "Explosion");
		if(null != gExplosion){
			cInsManager.DestroyInstancePool(gExplosion);
		}

		GameObject g = cInsManager.InstantiateInstancePool(v3Temp);
		if(null == g){
			return false;
		}
		g.transform.position = v3Temp;
		return true;
	}

}
