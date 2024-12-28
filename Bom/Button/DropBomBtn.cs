using UnityEngine;

public class DropBomBtn : LongPressButton {

    private PlayerAction cPlayerAction;
    public override void PushButton()
    {
        if(null == cPlayerAction){
            Field_Player_Base cField = GameObject.Find("Field").GetComponent<Field_Player_Base>();
            cPlayerAction = Library_Base.GetcPlayerActionFromObject(cField.GetPlayerName());
        }
        if(null != cPlayerAction){
            cPlayerAction.DropBom();
        }
    }
}