using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropBomBtn : LongPressButton {

    public override void PushButton()
    {
        Player cPlayer = FindAndSetPlayer();
        if (cPlayer != null)
        {
            cPlayer.GetPlayerAction().DropBom();
        }
    }



}