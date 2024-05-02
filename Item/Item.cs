using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerBomName;
using PlayerActionName;
using System.Text.RegularExpressions;
public class Item:MonoBehaviour{
    private SoundManager soundManager;

    void Awake(){
        soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }
    void Start(){
    }

    public virtual void Reflection(string objname){}


    private void OnTriggerEnter(Collider col){
        //Debug.Log($"{col.transform.name} is OnTriggerEnter");
        //if("SDunitychan1(Clone)" == col.transform.name || "SDunitychan2(Clone)" == col.transform.name){
        /*
        for (int i = 1; i <= 4; i++)
        {
            if((("Player"+i+"(Clone)")== col.transform.name)
            || (("PlayerOnline"+i+"(Clone)")== col.transform.name)
            || (("PlayerDummy"+i)== col.transform.name))
            {
                Reflection(col.transform.name);
                Destroy(this.gameObject);
            }
        }
        */

            if(col.transform.name.StartsWith("Player")){
                Reflection(col.transform.name);
                Destroy(this.gameObject);
                soundManager.PlaySoundEffect("GETITEM");
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
            else if(gPlayer.tag == "Player_CpuMode" || gPlayer.tag == "Player_DummyMode" ){
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
            else if(gPlayer.tag == "Player_CpuMode" || gPlayer.tag == "Player_DummyMode" ){
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

    protected Player GetcPlayerFromObject(string objname)
    {
        GameObject gPlayer = GameObject.Find(objname); // ゲームオブジェクトを検索
        Player cPlayer = null;

        if (gPlayer != null)
        {
            if(gPlayer.tag == "Player"){
                cPlayer = gPlayer.GetComponent<Player>();
            }
            else if(gPlayer.tag == "Player_CpuMode" || gPlayer.tag == "Player_DummyMode" ){
                cPlayer = gPlayer.GetComponent<Player_CpuMode>();
            }
            else if(gPlayer.tag == "Player_Online"){
                cPlayer = gPlayer.GetComponent<Player_Online>();
            }
        }
        else
        {
            Debug.LogError("Object with the name not found.");
        }

        return cPlayer;
    }

    protected static int ExtractNumberFromString(string input)
    {
        // 正規表現パターンを定義
        string pattern = @"\d+"; // 数字を表すパターン
        
        // 正規表現を使ってパターンにマッチする部分を抜き出す
        Match match = Regex.Match(input, pattern);
        
        // マッチした部分をint型に変換して返す
        if (match.Success)
        {
            return int.Parse(match.Value);
        }
        else
        {
            // マッチしなかった場合は0を返すが、適切なエラーハンドリングが必要であれば適宜修正してください
            return 0;
        }
    }

}
