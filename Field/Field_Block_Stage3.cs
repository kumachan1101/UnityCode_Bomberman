using UnityEngine;
using UnityEngine.UI;
public class Field_Block_Stage3 : Field_Block_CpuMode {
	protected override void SetFieldRange(){
		GameManager.SetFieldRange(12,12);
	}
    protected override void ConfigurePools()
    {
        AddPool(ExplosionTypes.Explosion1, 5000);
        AddPool(ExplosionTypes.Explosion3, 5000);
    }
}