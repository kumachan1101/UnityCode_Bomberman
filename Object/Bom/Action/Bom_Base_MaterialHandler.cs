using UnityEngine;

public class Bom_Base_MaterialHandler:  MonoBehaviour
{
    protected MaterialManager cMaterialMng;
    public string sMaterialKind;
    void Awake(){
        cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
    }

    void Start()
    {
        // インスタンス生成直後にマテリアルを設定している事もあり、Awakeのタイミングではまだマテリアルが取得できない。
        // 初回描画のタイミングであれば取得可能であるため、ここでマテリアルを設定している
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = GetComponentInChildren<Renderer>();
        }
        if (renderer != null)
        {
            cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
            Material newMaterial = cMaterialMng.GetMaterialOfType(sMaterialKind);
            renderer.material = newMaterial;
        }
        else
        {
            Debug.LogWarning("Renderer component not found on the game object or its children.");
        }
    }
    public void SetMaterialKind(string sParamMaterial){
        sMaterialKind = sParamMaterial;
    }

    public string GetExplotionString(){
        return cMaterialMng.GetMaterialOfExplosion(sMaterialKind);
    }

}