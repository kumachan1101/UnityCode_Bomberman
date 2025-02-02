public class BomConfigurationMaterialTypeManager : BomConfigurationManagerBase
{
    public BomConfigurationMaterialTypeManager() => configuration = new BomConfigurationMaterialType1();

    public override void Request(ReqType reqType){
        configuration = BomConfigurationFactory.Create(reqType);
    }
}

public class BomConfigurationMaterialType1 : BomConfigurationBase
{
    public BomConfigurationMaterialType1()
    {
        value = MaterialTypes.BomMaterial1;
    }
}

public class BomConfigurationMaterialType2 : BomConfigurationBase
{
    public BomConfigurationMaterialType2()
    {
        value = MaterialTypes.BomMaterial2;
    }
}

public class BomConfigurationMaterialType3 : BomConfigurationBase
{
    public BomConfigurationMaterialType3()
    {
        value = MaterialTypes.BomMaterial3;
    }
}

public class BomConfigurationMaterialType4 : BomConfigurationBase
{
    public BomConfigurationMaterialType4()
    {
        value = MaterialTypes.BomMaterial4;
    }
}
