using UnityEngine;
public class Field_Block_Stage1 : Field_Block_CpuMode {
	protected override void SetFieldRange(){
		GameManager.SetFieldRange(10,10);
	}

    protected override void ConfigurePools()
    {
        explosionManager.AddPool(ExplosionTypes.Explosion1, 500);
        explosionManager.AddPool(ExplosionTypes.Explosion2, 500);
        explosionManager.AddPool(ExplosionTypes.Explosion3, 500);
        explosionManager.AddPool(ExplosionTypes.Explosion4, 500);
    }
}