using UnityEngine;

public class DropBomBtn : LongPressButton {

    private PlayerAction cPlayerAction;
    public override void PushButton()
    {
        if(null == cPlayerAction){
            PlayerNameManager cField = GameObject.Find("Field").GetComponent<PlayerNameManager>();
            cPlayerAction = Library_Base.GetcPlayerActionFromObject(cField.GetPlayerName());
        }
        if(null != cPlayerAction){
            cPlayerAction.DropBom();
        }
    }
}