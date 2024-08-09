using UnityEngine;
public class Bom_CpuMode : Bom_Base
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

