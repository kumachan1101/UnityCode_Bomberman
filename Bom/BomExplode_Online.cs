using UnityEngine;

public class BomExplode_Online : Bom_Online
{
	protected override bool XorZ_Explosion(Vector3 v3Temp){
		bool bRet = IsWall(v3Temp);
		if(bRet){
			return true;
		}
		cLibrary.DeletePositionAndName(v3Temp, "Explosion");
		GameObject g = Instantiate_Explosion(v3Temp);
		g.transform.position = v3Temp;
		return false;
	}

}
