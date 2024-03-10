using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BomKind;
using PlayerBomName;

public class ItemExplodeBom : Item{
    public override void Reflection(string objname){
        PlayerBom cPlayerBom = GetPlayerBomFromObject(objname);
        if(null != cPlayerBom){
            cPlayerBom.SetBomKind(BomKind.BOM_KIND.BOM_KIND_EXPLODE);
        }
    }
}
