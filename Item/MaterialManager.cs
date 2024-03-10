using UnityEngine;

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
            case "BomMaterial1":
                return BomMaterial1;
            case "BomMaterial2":
                return BomMaterial2;
            case "BomMaterial3":
                return BomMaterial3;
            case "BomMaterial4":
                return BomMaterial4;
            default:
                Debug.LogError("Invalid material type: " + type);
                return null;
        }
    }
}