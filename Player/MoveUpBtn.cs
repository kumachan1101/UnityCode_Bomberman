using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveUpBtn : LongPressButton {
    public override void PushButton()
    {
        Player_Base cPlayer = FindAndSetPlayer();
        if (cPlayer != null)
        {
            cPlayer.GetPlayerAction().MoveUp();
        }
    }
    
}