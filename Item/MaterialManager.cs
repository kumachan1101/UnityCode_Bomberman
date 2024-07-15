using UnityEngine;

public static class MaterialTypes
{
    public const string BomMaterial1 = "BomMaterial1";
    public const string BomMaterial2 = "BomMaterial2";
    public const string BomMaterial3 = "BomMaterial3";
    public const string BomMaterial4 = "BomMaterial4";
}

public static class ExplosionTypes
{
    public const string Explosion1 = "Explosion1";
    public const string Explosion2 = "Explosion2";
    public const string Explosion3 = "Explosion3";
    public const string Explosion4 = "Explosion4";
}

public class MaterialManager : MonoBehaviour
{
    public Material BomMaterial1;
    public Material BomMaterial2;
    public Material BomMaterial3;
    public Material BomMaterial4;

    public Material GetMaterialOfType(string type)
    {
        switch (type)
        {
            case MaterialTypes.BomMaterial1:
                return BomMaterial1;
            case MaterialTypes.BomMaterial2:
                return BomMaterial2;
            case MaterialTypes.BomMaterial3:
                return BomMaterial3;
            case MaterialTypes.BomMaterial4:
                return BomMaterial4;
            default:
                Debug.LogError("Invalid material type: " + type);
                return null;
        }
    }

    public string GetMaterialOfExplosion(string type)
    {
        switch (type)
        {
            case MaterialTypes.BomMaterial1:
                return ExplosionTypes.Explosion1;
            case MaterialTypes.BomMaterial2:
                return ExplosionTypes.Explosion2;
            case MaterialTypes.BomMaterial3:
                return ExplosionTypes.Explosion3;
            case MaterialTypes.BomMaterial4:
                return ExplosionTypes.Explosion4;
            default:
                Debug.LogError("Invalid material type: " + type);
                return null;
        }
    }
}
