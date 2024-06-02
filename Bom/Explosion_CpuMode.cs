using UnityEngine;
public class Explosion_CpuMode : Explosion_Base
{
	protected override void DestroySync(GameObject g){
		Destroy(g);
	}

	protected override bool IsSync(){
		return true;
	}

}
