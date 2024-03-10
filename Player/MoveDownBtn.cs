using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveDownBtn : LongPressButton {

    public override void PushButton()
    {
        Player cPlayer = FindAndSetPlayer();
        if (cPlayer != null)
        {
            cPlayer.GetPlayerAction().BtnMoveDown();
        }
    }

    
}