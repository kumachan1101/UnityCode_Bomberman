using UnityEngine;

public class BomExplode_Base : Bom_Base
{
	
	protected override bool XorZ_Explosion(Vector3 v3Temp){
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

}
