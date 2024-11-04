using UnityEngine;

public enum BomConfigurationType
{
    ExplodeBom,
    BigBanBom,
    FireUp
}

public abstract class BomConfigurationBase
{
    protected int explosionNum;
    protected BOM_KIND bomKind;
    protected string materialType;

    public BomConfigurationBase()
    {
        explosionNum = 3;
        bomKind = BOM_KIND.BOM_KIND_NOTHING;
    }

    public BomConfigurationBase(BomConfigurationBase bomConfiguration){
        if(null == bomConfiguration){
            return;
        }
        explosionNum = bomConfiguration.GetExplosionNum();
        bomKind = bomConfiguration.GetBomKind();
        materialType = bomConfiguration.GetMaterialType();

    }


    // 派生クラスで実装される抽象メソッド
    public abstract void Request(BomConfigurationBase cBomConfiguration);

    // 共通プロパティのアクセサ
    public string GetMaterialType()
    {
        return materialType;
    }

    public void SetMaterialType(string materialType)
    {
        this.materialType = materialType;
    }

    public void SetExplosionNum(int explosionNum)
    {
        this.explosionNum = explosionNum;
    }
    public int GetExplosionNum()
    {
        return explosionNum;
    }

    public void SetBomKind(BOM_KIND bomKind){
        this.bomKind = bomKind;
    }

    public BOM_KIND GetBomKind()
    {
        return bomKind;
    }
}
