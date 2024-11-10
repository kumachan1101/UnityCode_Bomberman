using UnityEngine;
public class ItemRainbow : Item{
    public override void Reflection(string objname){
        PlayerBom cPlayerBom = Library_Base.GetPlayerBomFromObject(objname);
        if(null != cPlayerBom){
            string sMaterialType = cPlayerBom.GetMaterialType();
            Field_Block_Base cField = GameObject.Find("Field").GetComponent<Field_Block_Base>();
            cField.Rainbow_RPC(sMaterialType);
        }
    }
}
