public class DefaultBomConfiguration : BomConfigurationBase
{
    public override void Request(BomConfigurationBase cBomConfiguration)
    {
        cBomConfiguration.SetBomKind(BOM_KIND.BOM_KIND_NOTHING);
    }
}
