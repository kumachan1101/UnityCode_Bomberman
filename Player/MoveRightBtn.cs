using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MoveRightBtn : LongPressButton {
    public override void PushButton()
    {
        Player_Base cPlayer = Library_Base.FindAndSetPlayer();
        if (cPlayer != null)
        {
            cPlayer.GetPlayerAction().MoveRight();
        }
    }


}