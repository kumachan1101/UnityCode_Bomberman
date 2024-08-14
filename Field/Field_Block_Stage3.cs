using UnityEngine;
using UnityEngine.UI;
public class Field_Block_Stage3 : Field_Block_CpuMode {
	protected override void SetFieldRange(){
		GameManager.SetFieldRange(12,12);
	}
    protected override void ConfigurePools()
    {
        explosionManager.AddPool(ExplosionTypes.Explosion1, 5000);
        explosionManager.AddPool(ExplosionTypes.Explosion3, 5000);
    }
}