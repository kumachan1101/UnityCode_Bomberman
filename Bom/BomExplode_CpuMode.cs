using UnityEngine;

public class BomExplode_CpuMode : BomExplode_Base
{

	protected override void init(){
		cInsManager = gameObject.AddComponent<InstanceManager_CpuMode>();
		cInsManager.SetPrefab(ExplosionPrefab);		
	}

	protected override bool IsExplosion(){
		return true;
	}


}
