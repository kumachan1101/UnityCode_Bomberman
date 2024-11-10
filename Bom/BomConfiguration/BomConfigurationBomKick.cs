
public class BomConfigurationBomKick : BomConfigurationBase
{
    public BomConfigurationBomKick(){
        Set(false);
    }
    
    public override void Request()
    {
        Set(true);
    }
}
