using UnityEngine;
public class Bom_CpuMode : Bom_Base
{

	protected override GameObject Instantiate_Explosion(Vector3 v3){
		GameObject g = Instantiate(ExplosionPrefab);
		return g;
	}

}

