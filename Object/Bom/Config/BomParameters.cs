using UnityEngine;

[System.Serializable]
public class BomParameters
{
    public Vector3 position;
    public BOM_KIND bomKind;
    public int viewID;
    public int explosionNum;
    public bool bomKick;
    public string materialType;
    public BOM_ATTACK bomAttack;
    public Vector3 direction;
}

public class BomStatusData
{
    public bool bomKick;
    public BOM_ATTACK bomAttack;

    public BomStatusData()
    {
        this.bomKick = false;
        this.bomAttack = BOM_ATTACK.BOM_ATTACK_NOTHING;
    }

    public BomStatusData(BomParameters parameters)
    {
        this.bomKick = parameters.bomKick;
        this.bomAttack = parameters.bomAttack;
    }
}
