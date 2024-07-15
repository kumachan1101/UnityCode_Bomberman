using UnityEngine;

public class BomConfiguration
{
    private int explosionNum;
    private BOM_KIND bomKind;
    private string materialType;

    public BomConfiguration()
    {
        explosionNum = 3;
        bomKind = BOM_KIND.BOM_KIND_NOTHING;
    }

    public string GetMaterialType()
    {
        return materialType;
    }

    public void SetMaterialType(string materialType)
    {
        this.materialType = materialType;
    }

    public int GetExplosionNum()
    {
        return explosionNum;
    }

    public void IncreaseExplosion()
    {
        explosionNum++;
    }

    public BOM_KIND GetBomKind()
    {
        return bomKind;
    }

    public void SetBomKind(BOM_KIND bomKind)
    {
        this.bomKind = bomKind;
    }
}
