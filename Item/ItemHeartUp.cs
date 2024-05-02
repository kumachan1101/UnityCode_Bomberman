using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerActionName;
public class ItemHeartUp : Item{
    public override void Reflection(string objname){
        Player cPlayer = GetcPlayerFromObject(objname);
        if(null != cPlayer){
            cPlayer.HeartUp(5);
        }
    }
}
