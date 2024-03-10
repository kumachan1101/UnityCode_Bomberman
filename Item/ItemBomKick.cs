using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerBomName;
public class ItemBomKick : Item{
    

    public override void Reflection(string objname){
        PlayerBom cPlayerBom = GetPlayerBomFromObject(objname);
        if(null != cPlayerBom){
            cPlayerBom.BomKick();
        }
    }


}
