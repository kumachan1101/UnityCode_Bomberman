using UnityEngine;

public class PlayerMaterial
{
    private Material cMaterial;

    public void SetMaterialType(string sParamMaterialType)
    {
        MaterialManager cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
        cMaterial = cMaterialMng.GetMaterialOfType(sParamMaterialType);
    }

    public Material GetMaterial()
    {
        return cMaterial;
    }
}
