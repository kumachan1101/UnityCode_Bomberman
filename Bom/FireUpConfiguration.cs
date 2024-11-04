public class FireUpConfiguration : BomConfigurationBase
{
    public override void Request(BomConfigurationBase cBomConfiguration)
    {
        // 派生クラスで特定の処理: 爆発範囲を増加
        int iExplosionNum = cBomConfiguration.GetExplosionNum();
        iExplosionNum++;
        cBomConfiguration.SetExplosionNum(iExplosionNum);
    }
}
