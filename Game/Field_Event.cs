using UnityEngine;
abstract public class Field_Event :MonoBehaviour{

    void Start()
    {
        Init();
        RegisterListeners();
    }
    void OnDestroy()
    {
        // オブジェクトが破棄される際にリスナーを解除
        UnregisterListeners();
    }
    // イベントリスナーの登録
    protected abstract void RegisterListeners();
    protected abstract void UnregisterListeners();


    // 派生クラスで必要に応じて初期化処理を定義
    protected virtual void Init() { }
}