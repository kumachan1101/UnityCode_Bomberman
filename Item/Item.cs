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
        Player_Base cPlayer = GetcPlayerFromObject(objname);
        if (cPlayer != null)
        {
            cPlayerBom = cPlayer.GetPlayerBom();
        }
        return cPlayerBom;
    }

    protected PlayerAction GetcPlayerActionFromObject(string objname)
    {
        GameObject gPlayer = GameObject.Find(objname); // ゲームオブジェクトを検索
        PlayerAction cPlayerAction = null;
        Player_Base cPlayer = GetcPlayerFromObject(objname);
        if (cPlayer != null)
        {
            cPlayerAction = cPlayer.GetPlayerAction();
        }
        return cPlayerAction;
    }

    protected Player_Base GetcPlayerFromObject(string objname)
    {
        GameObject gPlayer = GameObject.Find(objname); // ゲームオブジェクトを検索
        Player_Base cPlayer = null;
        if (gPlayer == null)
        {
            Debug.LogError("Object with the name not found.");
            return cPlayer;
        }
        cPlayer = gPlayer.GetComponent<Player>();
        if(null == cPlayer){
            cPlayer = gPlayer.GetComponent<Player_CpuMode>();
        }
        if(null == cPlayer){
            cPlayer = gPlayer.GetComponent<Player_Online>();
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
