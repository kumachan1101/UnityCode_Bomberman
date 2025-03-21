/*
public class BomConfigurationFireUpManager : BomConfigurationManagerBase
{
    public BomConfigurationFireUpManager() => configuration = new BomConfigurationFireUp();

    public override void Request(ReqType reqType){
        configuration.Request();
    }

}
*/
public class BomConfigurationFireUpManager : BomConfigurationManagerBase
{
    public BomConfigurationFireUpManager() => configuration = BomConfigurationFactory.Create(ReqType.FireUp);
}


public class BomConfigurationFireUp : BomConfigurationBase
{
    public BomConfigurationFireUp(){
        value = 3;
    }
    public override void Request()
    {
        // 派生クラスで特定の処理: 爆発範囲を増加
        int iExplosionNum = (int)Get();
        iExplosionNum++;
        value = iExplosionNum;
    }
}

