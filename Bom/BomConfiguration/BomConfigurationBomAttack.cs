

public class BomConfigurationBomAttack : BomConfigurationBase
{
    public BomConfigurationBomAttack(){
        Set(false);
    }
    public override void Request()
    {
        Set(true);
    }
}
