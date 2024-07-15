using UnityEngine;

public class ItemAddDummy : Item{
    public override void Reflection(string objname){
        //objnameと同じ種類のリソースを生成する
        //tagとコンポーネントはCPUMODEに変更
        //座標はアイテム取得者と同じ座標を設定する
        GameObject gPlayer = GameObject.Find(objname); // ゲームオブジェクトを検索
        Field_Player_Base cField = GameObject.Find("Field").GetComponent<Field_Player_Base>();
        int playercnt = Library_Base.ExtractNumberFromString(objname);
        cField.AddDummyPlayer(playercnt, gPlayer.transform.position);
    }
}
