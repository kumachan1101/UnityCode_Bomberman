using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerBomName;
using PlayerActionName;

public class Item:MonoBehaviour{
    public Item(){
        
    }

    public virtual void Reflection(string objname){}


    private void OnTriggerEnter(Collider col){
        //Debug.Log($"{col.transform.name} is OnTriggerEnter");
        //if("SDunitychan1(Clone)" == col.transform.name || "SDunitychan2(Clone)" == col.transform.name){
        for (int i = 1; i <= 4; i++)
        {
            if((("Player"+i+"(Clone)")== col.transform.name)
            || (("PlayerOnline"+i+"(Clone)")== col.transform.name)){
                Reflection(col.transform.name);
                Destroy(this.gameObject);
            }
        }
    }

    protected PlayerBom GetPlayerBomFromObject(string objname)
    {
        GameObject gPlayer = GameObject.Find(objname); // ゲームオブジェクトを検索
        PlayerBom cPlayerBom = null; // PlayerBomコンポーネントの参照を初期化

        if (gPlayer != null)
        {
            Player cPlayer = null;
            if(gPlayer.tag == "Player"){
                cPlayer = gPlayer.GetComponent<Player>();
            }
            else if(gPlayer.tag == "Player_CpuMode"){
                cPlayer = gPlayer.GetComponent<Player_CpuMode>();
            }
            else if(gPlayer.tag == "Player_Online"){
                cPlayer = gPlayer.GetComponent<Player_Online>();
            }

            if (cPlayer != null)
            {
                cPlayerBom = cPlayer.GetPlayerBom();
            }
        }
        else
        {
            Debug.LogError("Object with the name not found.");
        }
        //Debug.Log("objname: "+objname + "cPlayerBom :" + cPlayerBom);
        return cPlayerBom;
    }

    protected PlayerAction GetcPlayerActionFromObject(string objname)
    {
        GameObject gPlayer = GameObject.Find(objname); // ゲームオブジェクトを検索
        PlayerAction cPlayerAction = null;

        if (gPlayer != null)
        {
            Player cPlayer = null;
            if(gPlayer.tag == "Player"){
                cPlayer = gPlayer.GetComponent<Player>();
            }
            else if(gPlayer.tag == "Player_CpuMode"){
                cPlayer = gPlayer.GetComponent<Player_CpuMode>();
            }
            else if(gPlayer.tag == "Player_Online"){
                cPlayer = gPlayer.GetComponent<Player_Online>();
            }

            if (cPlayer != null)
            {
                cPlayerAction = cPlayer.GetPlayerAction();
            }
        }
        else
        {
            Debug.LogError("Object with the name not found.");
        }

        return cPlayerAction;
    }



}
