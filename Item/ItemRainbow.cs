using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerBomName;
public class ItemRainbow : Item{
    public override void Reflection(string objname){
        PlayerBom cPlayerBom = GetPlayerBomFromObject(objname);
        if(null != cPlayerBom){
            string sMaterialType = cPlayerBom.GetMaterialType();
            Field_Base cField = GameObject.Find("Field").GetComponent<Field_Base>();
            cField.Rainbow(sMaterialType);
        }
    }
}
