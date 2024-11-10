

public class BomConfigurationBomUp : BomConfigurationBase
{
    public BomConfigurationBomUp(){
        Set(3);
    }
    public override void Request()
    {
        int ivalue = (int)Get();
        ivalue++;
        Set(ivalue);
    }
}
