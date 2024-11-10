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
        // シングルトンパターンで、既存のインスタンスがある場合は自分自身を破棄
        if (instance == null)
        {
            // このインスタンスを保存し、DontDestroyOnLoadで破棄されないようにする
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 既に存在するインスタンスがある場合、このGameObjectを破棄
            Destroy(gameObject);
        }
    }
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


    public Material GetMaterialOfTypeExplosion(string type)
    {
        switch (type)
        {
            case ExplosionTypes.Explosion1:
                return BomMaterial1;
            case ExplosionTypes.Explosion2:
                return BomMaterial2;
            case ExplosionTypes.Explosion3:
                return BomMaterial3;
            case ExplosionTypes.Explosion4:
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

    public ReqType GetReqtypeOfMaterial(string type)
    {
        switch (type)
        {
            case MaterialTypes.BomMaterial1:
                return ReqType.MaterialBom1;
            case MaterialTypes.BomMaterial2:
                return ReqType.MaterialBom2;
            case MaterialTypes.BomMaterial3:
                return ReqType.MaterialBom3;
            case MaterialTypes.BomMaterial4:
                return ReqType.MaterialBom4;
            default:
                Debug.LogError("Invalid material type: " + type);
                return ReqType.MaterialBom1;
        }
    }

    public string GetBomMaterialByPlayerName(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogError("Player name cannot be null or empty");
            return null;
        }
        
        string suffix = playerName.Replace("Player", "").Replace("Dummy", "").Replace("Online", "").Replace("(Clone)", "");

        if (int.TryParse(suffix, out int playerNumber))
        {
            switch (playerNumber)
            {
                case 1:
                    return MaterialTypes.BomMaterial1;
                case 2:
                    return MaterialTypes.BomMaterial2;
                case 3:
                    return MaterialTypes.BomMaterial3;
                case 4:
                    return MaterialTypes.BomMaterial4;
                default:
                    Debug.LogError("Invalid player number: " + playerNumber);
                    return null;
            }
        }
        else
        {
            Debug.LogError("Invalid player name format: " + playerName);
            return null;
        }
    }
}
