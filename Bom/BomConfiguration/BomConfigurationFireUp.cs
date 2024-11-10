public class BomConfigurationFireUp : BomConfigurationBase
{
    public BomConfigurationFireUp(){
        Set(3);
    }
    public override void Request()
    {
        // 派生クラスで特定の処理: 爆発範囲を増加
        int iExplosionNum = (int)Get();
        iExplosionNum++;
        Set(iExplosionNum);
    }
}
