using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveRightBtn : LongPressButton {
    public override void PushButton()
    {
        Player cPlayer = FindAndSetPlayer();
        if (cPlayer != null)
        {
            cPlayer.GetPlayerAction().BtnMoveRight();
        }
    }


}