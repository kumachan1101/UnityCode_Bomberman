public class BigBanBomConfiguration : BomConfigurationBase
{
    public override void Request(BomConfigurationBase cBomConfiguration)
    {
        cBomConfiguration.SetBomKind(BOM_KIND.BOM_KIND_BIGBAN);
    }
}
