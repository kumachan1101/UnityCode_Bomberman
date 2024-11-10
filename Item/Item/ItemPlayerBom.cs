public abstract class ItemPlayerBom : Item
{
    // 抽象メソッドを定義し、各派生クラスで BomConfigurationType を指定
    protected abstract ReqType GetReqType();

    // 共通処理を持つ Reflection メソッドを基底クラスに移動
    public override void Reflection(string objname)
    {
        PlayerBom cPlayerBom = Library_Base.GetPlayerBomFromObject(objname);
        cPlayerBom.Request(GetReqType());
    }
}
