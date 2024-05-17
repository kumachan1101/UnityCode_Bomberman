using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveLeftBtn : LongPressButton {
    public override void PushButton()
    {
        Player_Base cPlayer = FindAndSetPlayer();
        if (cPlayer != null)
        {
            cPlayer.GetPlayerAction().MoveLeft();
        }
    }

    
}