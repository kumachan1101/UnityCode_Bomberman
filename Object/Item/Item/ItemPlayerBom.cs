//using System.Diagnostics;
using UnityEngine;

public abstract class ItemPlayerBom : Item
{
    // 抽象メソッドを定義し、各派生クラスで BomConfigurationType を指定
    protected abstract ReqType GetReqType();

    // 共通処理を持つ Reflection メソッドを基底クラスに移動
    public override void Reflection(GameObject gObj)
    {
        PlayerBom cPlayerBom = gObj.GetComponent<PlayerBom>();
        if(null != cPlayerBom){
            cPlayerBom.Request(GetReqType());
        }
    }
}
