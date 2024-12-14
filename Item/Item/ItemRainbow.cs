using UnityEngine;
public class ItemRainbow : Item{
    public override void Reflection(GameObject gObj){
        PlayerBom cPlayerBom = gObj.GetComponent<PlayerBom>();
        if(null != cPlayerBom){
            string sMaterialType = cPlayerBom.Get<string>(GetKind.MaterialType);
            MaterialManager cMaterialMng = GameObject.Find("MaterialManager").GetComponent<MaterialManager>();
            Field_Block_Base cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
            cField.Rainbow_RPC(cMaterialMng.GetMaterialOfExplosion(sMaterialType));
        }
    }
}
