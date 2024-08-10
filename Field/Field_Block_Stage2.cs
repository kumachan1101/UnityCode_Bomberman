
public class Field_Block_Stage2 : Field_Block_CpuMode {
	protected override void SetFieldRange(){
		GameManager.SetFieldRange(15,15);
	}
    protected override void ConfigurePools()
    {
        AddPool(ExplosionTypes.Explosion1, 5000);
        AddPool(ExplosionTypes.Explosion2, 5000);
    }
}