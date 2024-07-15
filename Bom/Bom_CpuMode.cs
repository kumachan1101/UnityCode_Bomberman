public class Bom_CpuMode : Bom_Base
{
	protected override void init(){
		cInsManager = gameObject.AddComponent<InstanceManager_CpuMode>();
		cInsManager.SetPrefab(ExplosionPrefab);		
	}

	protected override bool IsExplosion(){
		return true;
	}

}

