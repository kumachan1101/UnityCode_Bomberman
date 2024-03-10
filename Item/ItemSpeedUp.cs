using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerActionName;
public class ItemSpeedUp : Item{
    public override void Reflection(string objname){
        PlayerAction cPlayerAction = GetcPlayerActionFromObject(objname);
        if(null != cPlayerAction){
            cPlayerAction.SpeedUp();
        }
    }
}
