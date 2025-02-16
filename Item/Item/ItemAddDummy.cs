using UnityEngine;

public class ItemAddDummy : Item{
    public override void Reflection(GameObject gObj){
        //objnameと同じ種類のリソースを生成する
        //tagとコンポーネントはCPUMODEに変更
        //座標はアイテム取得者と同じ座標を設定する
        PlayerSpawnManager cField = GameObject.Find("Field").GetComponent<PlayerSpawnManager>();
        int playercnt = Library_Base.ExtractNumberFromString(gObj.name);
        cField.AddDummyPlayer(playercnt, gObj.transform.position);
    }
}
