using UnityEngine;
using UnityEngine.UI;
public class Field_Block_Stage4 : Field_Block_CpuMode {
	protected override void SetFieldRange(){
		GameManager.SetFieldRange(15,15);
	}
    protected override void ConfigurePools()
    {
        AddPool(ExplosionTypes.Explosion1, 5000);
        AddPool(ExplosionTypes.Explosion4, 5000);
    }
}