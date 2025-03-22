
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
    private static MaterialManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static MaterialManager Instance => instance;

    public Material GetMaterial(string type)
    {
        return type switch
        {
            MaterialTypes.BomMaterial1 => BomMaterial1,
            MaterialTypes.BomMaterial2 => BomMaterial2,
            MaterialTypes.BomMaterial3 => BomMaterial3,
            MaterialTypes.BomMaterial4 => BomMaterial4,
            _ => null
        };
    }
}

public static class MaterialMapper
{
    public static string GetExplosionType(string materialType)
    {
        return materialType switch
        {
            MaterialTypes.BomMaterial1 => ExplosionTypes.Explosion1,
            MaterialTypes.BomMaterial2 => ExplosionTypes.Explosion2,
            MaterialTypes.BomMaterial3 => ExplosionTypes.Explosion3,
            MaterialTypes.BomMaterial4 => ExplosionTypes.Explosion4,
            _ => null
        };
    }

    public static ReqType GetReqtypeOfMaterial(string materialType)
    {
        return materialType switch
        {
            MaterialTypes.BomMaterial1 => ReqType.MaterialBom1,
            MaterialTypes.BomMaterial2 => ReqType.MaterialBom2,
            MaterialTypes.BomMaterial3 => ReqType.MaterialBom3,
            MaterialTypes.BomMaterial4 => ReqType.MaterialBom4,
            _ => ReqType.MaterialBom1
        };
    }
}

public static class MaterialResolver
{
    public static string GetBomMaterialByPlayerName(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("Player name cannot be null or empty");
            return null;
        }
        
        string suffix = playerName.Replace("Player", "")
                                  .Replace("Dummy", "")
                                  .Replace("Online", "")
                                  .Replace("(Clone)", "")
                                  .Replace("(Instance)", "")
                                  .Replace("BomMaterial", "")
                                  .Replace("(Material)", "")
                                  .Replace("Tower", "");

        return int.TryParse(suffix, out int playerNumber) ? playerNumber switch
        {
            1 => MaterialTypes.BomMaterial1,
            2 => MaterialTypes.BomMaterial2,
            3 => MaterialTypes.BomMaterial3,
            4 => MaterialTypes.BomMaterial4,
            _ => null
        } : null;
    }
}
