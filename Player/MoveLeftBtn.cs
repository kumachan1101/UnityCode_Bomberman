using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveLeftBtn : LongPressButton {
    public override void PushButton()
    {
        Player cPlayer = FindAndSetPlayer();
        if (cPlayer != null)
        {
            cPlayer.GetPlayerAction().BtnMoveLeft();
        }
    }

    
}