using UnityEngine;

public class BomExplode_CpuMode : BomExplode_Base
{

	protected override void init(){
		cInsManager = gameObject.AddComponent<InstanceManager_CpuMode>();
		sExplosion = cMaterialMng.GetMaterialOfExplosion(sMaterialKind);
		base.init();
	}

	protected override bool IsExplosion(){
		return true;
	}


}
