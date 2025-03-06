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
    public bool bomAttack;
    public Vector3 direction;
}
