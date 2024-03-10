using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWall : Item{
    
    public override void Reflection(string objname){
        GameObject gPlayer =  GameObject.Find(objname);
        gPlayer.GetComponent<Player_CpuMode>().Wall();
    }

}
